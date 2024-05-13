using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Clankboard.AppInfobar;

namespace Clankboard
{
    public class AppMessagebox
    {
        public delegate Task<int> CustomButtonClickEventHandler(object sender, RoutedEventArgs e, string Title, string Text, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText, ContentDialogButton DefaultButton = ContentDialogButton.None, object content = null);
        public event CustomButtonClickEventHandler NewMessageBox;

        public Task<int> ShowMessagebox(string title, string text, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText, ContentDialogButton DefaultButton = ContentDialogButton.None, object content = null)
        {
            return NewMessageBox(this, null, title, text, PrimaryButtonText, SecondaryButtonText, CloseButtonText, DefaultButton, content);
        }
    }

    public class AppInfobar
    {
        public enum AppInfobarType
        {
            FileDownloadInfobar,
            DriverMissingInfobar,
            FileMissingInfobar,
            DebugModeInfobar
        }

        public delegate void InfobarEventHandler(object sender, RoutedEventArgs e, AppInfobarType type, bool Open);
        public event InfobarEventHandler OpenInfobar;
        public void OpenAppInfobar(AppInfobarType type, bool Open = true) => OpenInfobar(this, null, type, Open);
    }

    public class ShellPageEvents
    {
        public event RoutedEventHandler SettingsOpenClick;
        public void OpenAppSettings(object sender) => SettingsOpenClick(sender, null);
    }

    public class SoundboardEvents
    {
        #region Soundboard: Add File
        public delegate void SoundboardAddFile(object sender, RoutedEventArgs e, string Name, string Path);
        public event SoundboardAddFile NewSoundboardItem_FILE;
        public void AddFile(string Name, string Path) => NewSoundboardItem_FILE(this, null, Name, Path);
        #endregion

        #region Soundboard: Add URL
        public delegate void SoundboardAddURL(object sender, RoutedEventArgs e, string Name, string Path);
        public event SoundboardAddURL NewSoundboardItem_URL;
        public void AddFileURL(string Name, string URL) => NewSoundboardItem_URL(this, null, Name, URL);
        #endregion

        public event EventHandler DeleteAllSoundboardItems;
        public void DeleteAllItems() => DeleteAllSoundboardItems(this, null);
    }

    public static class FileSaveHandler
    {
        private static string CurrentOpenFilePath;

        public enum SoundboardEntryType
        {
            File,
            URL
        }

        public struct SoundboardFileEntry
        {
            public string Name;
            public string Path;
            public SoundboardEntryType Type;
        }

        public static void OpenFile(string FilePath)
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException();

            CurrentOpenFilePath = FilePath;
            // TODO: Add file open handler
        }
    }
}
