using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dropbox.Api;
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

            Task<string> user  = Task.Run((Func<Task<string>>)DropboxManager.Run);
            


            var result = new Result
            {
                Title = "Dropbox User: " + user.Result,
                SubTitle = $"Query: {query.Search}",
                IcoPath = "app.png"
            };

            return new List<Result> { result };
        }

    }
}