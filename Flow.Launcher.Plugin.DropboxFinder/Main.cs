using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Flow.Launcher.Plugin;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;

namespace Flow.Launcher.Plugin.DropboxFinder
{

    /// <summary>
    /// Main Plugin Class
    /// </summary>
    public class DropboxFinder : IPlugin, ISettingProvider
    {


        /// <summary>
        /// Dropbox snippets to enable PKCE OAuth flow
        /// </summary>
        public const string AppName = "vayv5p3x303bxax";
        public static string RefreshToken = "";

        // This loopback host is for demo purpose. If this port is not
        // available on your machine you need to update this URL with an unused port.
        private const string LoopbackHost = "http://127.0.0.1:52475/";

        // URL to receive OAuth 2 redirect from Dropbox server.
        // You also need to register this redirect URL on https://www.dropbox.com/developers/apps.
        private readonly Uri RedirectUri = new Uri(LoopbackHost + "authorize");

        // URL to receive access token from JS.
        private readonly Uri JSRedirectUri = new Uri(LoopbackHost + "token");


        private PluginInitContext _context;

        private static Settings settings;
        

        public void Init(PluginInitContext context)
        {
            _context = context;
            
            
            //Prepare settings
            var settingsFolderLocation =
                Path.Combine(
                    Directory.GetParent(
                        Directory.GetParent(context.CurrentPluginMetadata.PluginDirectory).FullName)
                    .FullName,
                    "Settings", "Plugins", "Flow.Launcher.Plugin.DropboxFinder");

            var settingsFileLocation = Path.Combine(settingsFolderLocation, "Settings.json");

            if (!Directory.Exists(settingsFolderLocation))
            {
                Directory.CreateDirectory(settingsFolderLocation);

                settings = new Settings
                {
                    SettingsFileLocation = settingsFileLocation
                };

                settings.Save();
            }
            else
            {
                settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsFileLocation));
                settings.SettingsFileLocation = settingsFileLocation;
            }

            RefreshToken = settings.OAuthRefreshToken;

        }

        public List<Result> Query(Query query)
        {

            //Check the token being available
            if (string.IsNullOrEmpty(RefreshToken))
            {

                return new List<Result>{ new Result {

                    Title = "API key not setup, click to setup.",
                    //SubTitle = "",
                    //IcoPath = "app.png",
                    Action = context => SetupOAuth()
                    }

                };

            }


            //Token exists and validated, let's use it
            if (query.Search == "")
                return new List<Result>();


            Task<List<SearchMatchV2>> matches = Task.Run(() => DropboxManager.GetRelevantItems(query.Search, 25));

            //Check token validity
            try
            {

                matches.Wait();

            }
            catch (Exception ex)
            {

                //if (ex.Message.Contains("expired_access_token"))

                return new List<Result>{ new Result {

                    Title = ex.Message,
                    //SubTitle = "",
                    //IcoPath = "app.png",
                    //Action = context => SetupOAuth()
                    }

                };

            }


            if (matches.Result.Count != 0)
            {

                var resultList = new List<Result>();

                foreach (SearchMatchV2 match in matches.Result)
                {


                    var result = new Result
                    {
                        Title = match.Metadata.AsMetadata.Value.Name,
                        SubTitle = match.Metadata.AsMetadata.Value.PathDisplay,
                        IcoPath = "app.png",
                        Action = context => NavigateToFolder(match.Metadata.AsMetadata.Value.PathDisplay)

                    };

                    resultList.Add(result);

                }

                return resultList;
                
            }

            return new List<Result>{ new Result {

                Title = "Nothing found..",
                //SubTitle = "",
                //IcoPath = "app.png",
                }

            };

        }

        /// <summary>
        /// Quick hack to open a locally available file (for now in E:)
        /// </summary>
        /// <param name="subTitle"></param>
        /// <returns></returns>
        private bool NavigateToFolder(string subTitle)
        {
            System.Diagnostics.Process.Start("explorer.exe", settings.DropboxFolderPath + subTitle.Replace('/', '\\'));
            return true;
        }

        private bool SetupOAuth()
        {

            //OAuth PKCE snippets taken mostly from Dropbox sample code for .NET
            var state = Guid.NewGuid().ToString("N");
            var OAuthFlow = new PKCEOAuthFlow();
            var authorizeUri = OAuthFlow.GetAuthorizeUri(OAuthResponseType.Code, AppName, RedirectUri.ToString(), state: state, tokenAccessType: TokenAccessType.Offline);

            //Lousy and quick logging, don't judge :)
            //File.WriteAllText(@"C:\test\DFLog.txt", "URL is " + authorizeUri.ToString() + Environment.NewLine);
            
            var http = new HttpListener();
            http.Prefixes.Add(LoopbackHost);

            http.Start();

            //Process.Start() fails on DonNET Code, this code does the job
            OpenBrowser(authorizeUri.ToString());

            // Handle OAuth redirect and send URL fragment to local server using JS.
            Task<bool> res = Task.Run(() => HandleOAuth2Redirect(http));
            res.Wait();

            //// Handle redirect from JS and process OAuth response.
            Task<Uri> res2 = Task.Run(() => HandleJSRedirect(http));
            res2.Wait();

            var redirectUri = res2.Result;
            var tokenResult = Task.Run(() => OAuthFlow.ProcessCodeFlowAsync(redirectUri, AppName, RedirectUri.ToString(), state));
            tokenResult.Wait();

            RefreshToken = tokenResult.Result.RefreshToken;

            //Keep the token to be used for further sessions
            //File.WriteAllText(@"C:\test\refreshtoken.cfg", RefreshToken);
            settings.OAuthRefreshToken = RefreshToken;
            settings.Save();

            http.Stop();
            

            return true;
        }


        //Process.Start() fails on DonNET Code, this code does the job
        //Courtesy of https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<bool> HandleOAuth2Redirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to RedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            context.Response.ContentType = "text/html";

            // Respond with a page which runs JS and sends URL fragment as query string
            // to TokenRedirectUri.

            //Embedded the redirect page instead of a having a lingering html file
            string s = "<html><script type = \"text/javascript\" > function redirect() {document.location.href = \"/token?url_with_fragment=\" + encodeURIComponent(document.location.href);}</script><body onload = \"redirect()\" /> </ html > \"";
            byte[] byteArray = Encoding.ASCII.GetBytes(s);
            MemoryStream stream = new MemoryStream(byteArray);

            stream.CopyTo(context.Response.OutputStream);
            stream.Close();

            context.Response.OutputStream.Close();

            return true;
        }

        /// <summary>
        /// Handle the redirect from JS and process raw redirect URI with fragment to
        /// complete the authorization flow.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="OAuth2Response"/></returns>
        private async Task<Uri> HandleJSRedirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to TokenRedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != JSRedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);

            return redirectUri;
        }

        public Control CreateSettingPanel()
        {
            return new DropboxFinderSettings(settings);
        }

    }
}