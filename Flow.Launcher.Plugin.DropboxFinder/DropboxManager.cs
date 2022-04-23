using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.DropboxFinder
{
    class DropboxManager
    {

        public static async Task<string> Run()
        {
            using (var dbx = new DropboxClient(File.ReadAllText(@"C:\token.cfg")))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                return full.Email;
            }
        }


    }
}
