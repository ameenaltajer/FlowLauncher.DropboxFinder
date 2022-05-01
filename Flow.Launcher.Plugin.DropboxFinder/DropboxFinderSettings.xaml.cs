using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Flow.Launcher.Plugin;
using Flow.Launcher.Plugin.DropboxFinder;
using Microsoft.Win32;
using System.IO;

namespace Flow.Launcher.Plugin.DropboxFinder
{
    /// <summary>
    /// Interaction logic for DropboxFinderSettings.xaml
    /// </summary>
    public partial class DropboxFinderSettings : UserControl
    {
        private readonly Settings _settings;

        public DropboxFinderSettings(Settings settings)
        {
           
            InitializeComponent();
            this._settings = settings;
        }

        private void DropboxFinderSettings_Loaded(object sender, RoutedEventArgs e)
        {

            OAuthRefreshKey.Text = _settings.OAuthRefreshToken;


        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            _settings.OAuthRefreshToken = OAuthRefreshKey.Text;
            _settings.Save();
            
            
        }

        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {

            if (DropboxFolderPath.Text.Length != 0 && DropboxFolderPath.Text[DropboxFolderPath.Text.Length - 1] == '\\')
                DropboxFolderPath.Text = DropboxFolderPath.Text.Substring(0, DropboxFolderPath.Text.Length - 1);

            _settings.DropboxFolderPath = DropboxFolderPath.Text;
            _settings.Save();


        }

    }


}