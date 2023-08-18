using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard
{
    public class AppMessagebox
    {
        public delegate Task<int> CustomButtonClickEventHandler(object sender, RoutedEventArgs e, string Title, string Text, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText, ContentDialogButton DefaultButton = ContentDialogButton.None);
        public event CustomButtonClickEventHandler NewMessageBox;

        public Task<int> ShowMessagebox(string title, string text, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText, ContentDialogButton DefaultButton = ContentDialogButton.None)
        {
            return NewMessageBox(this, null, title, text, PrimaryButtonText, SecondaryButtonText, CloseButtonText, DefaultButton);
        }
    }

    public class SoundboardEvents
    {
        public delegate void SoundboardAddFile(object sender, RoutedEventArgs e, string Name, string Path);
        public event SoundboardAddFile NewSoundboardItem_FILE;

        public void AddFile(string Name, string Path)
        {
            NewSoundboardItem_FILE(this, null, Name, Path);
        }

        public event EventHandler DeleteAllSoundboardItems;
        public void DeleteAllItems()
        {
            DeleteAllSoundboardItems(this, null);
        }
    }
}
