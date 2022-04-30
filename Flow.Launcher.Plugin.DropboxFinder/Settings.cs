using System;
using System.IO;
using System.Text.Json;

namespace Flow.Launcher.Plugin.DropboxFinder
{
    public class Settings
    {
        internal string SettingsFileLocation;

        public string OAuthRefreshToken { get; set; }

        internal void Save()
        {
            File.WriteAllText(SettingsFileLocation, JsonSerializer.Serialize(this));
        }
    }
}