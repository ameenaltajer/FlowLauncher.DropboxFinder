using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api.Files;

namespace Flow.Launcher.Plugin.DropboxFinder
{
    class DropboxManager
    {

        public static async Task<string> GetEmail()
        {
            using (var dbx = new DropboxClient(File.ReadAllText(@"C:\token.cfg")))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                return full.Email;
            }
        }

        public static async Task<List<SearchMatchV2>> GetRelevantItems(string item, ulong count)
        {
            using (var dbx = new DropboxClient(File.ReadAllText(@"C:\token.cfg")))
            {
                var result = await dbx.Files.SearchV2Async(new Dropbox.Api.Files.SearchV2Arg(item, new Dropbox.Api.Files.SearchOptions(null, count)));

                return result.Matches.ToList();

            }
        }


    }
}
