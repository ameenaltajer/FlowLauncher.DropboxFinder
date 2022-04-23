using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.DropboxFinder
{
    public class DropboxFinder : IPlugin
    {
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;


        }

        public List<Result> Query(Query query)
        {

            if (query.Search == "")
                return new List<Result>();

            Task<List<SearchMatchV2>> matches = Task.Run(() => DropboxManager.GetRelevantItems(query.Search, 10));

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

        private bool NavigateToFolder(string subTitle)
        {
            System.Diagnostics.Process.Start("explorer.exe", @"E:\Dropbox" + subTitle.Replace('/', '\\'));
            return true;
        }

    }
}