using Clankboard.AudioSystem;
using Clankboard.Dialogs;
using Clankboard.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SoundboardPage : Page
    {
        private Soundboard soundBoard = new Soundboard();

        public SoundboardPage()
        {
            this.InitializeComponent();

            // Set data source for the soundboard list view
            SoundboardListView.ItemsSource = soundBoard.soundboardViewmodel.SoundboardItems;
            soundBoard.soundboardViewmodel.SoundboardItems.CollectionChanged += SoundboardItems_CollectionChanged;

            // Add random soundboard items for testing
            soundBoard.Add(new SoundboardItem("Test Item", @"C:\Windows\Windows.mp3", SoundboardItemType.LocalFile, "", true, false));
            soundBoard.Add(new SoundboardItem("Test Item 2", @"C:\Windows\Windows.mp3", SoundboardItemType.LocalFile, "", true, false, true));
            soundBoard.Add(new SoundboardItem("Test Item with really really looooooooong name oooo soo long", @"C:\Users\Really\Long\File\Path\That\Exceeds\The\Max\Width\Of\Display.mp3", SoundboardItemType.LocalFile, "", true, false));
            soundBoard.Add(new SoundboardItem("Test Item Downloaded Item", @"https://www.youtube.com/watch?v=WyQ7z8BMwwk", SoundboardItemType.DownloadedFile, "", true, false, false));
            soundBoard.Add(new SoundboardItem("Test Item Downloading Item", @"https://www.youtube.com/watch?v=WyQ7z8BMwwk", SoundboardItemType.DownloadedFile, "", false, true, false));
            soundBoard.Add(new SoundboardItem("Test Item Downloaded Item w/ Errors", @"https://www.youtube.com/watch?v=WyQ7z8BMwwk", SoundboardItemType.DownloadedFile, "", true, false, true));
            soundBoard.Add(new SoundboardItem("Test TTS Item", "Hi, This is some test TTS Text!", SoundboardItemType.TTSFile, "", true, false, false));
            soundBoard.Add(new SoundboardItem("Test TTS Item w/ Errors", "Hi, This is some test TTS Text!", SoundboardItemType.TTSFile, "", true, false, true));
        }

        private void SoundboardItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Check if the soundboard is empty. If it is, show the empty soundboard message, otherwise hide it.
            if (soundBoard.soundboardViewmodel.SoundboardItems.Count == 0)
            {
                NoItemsDisplay.Visibility = Visibility.Visible;
            }
            else
            {
                NoItemsDisplay.Visibility = Visibility.Collapsed;
            }
        }

        private async void AddLocalSoundFile_Click(object sender, RoutedEventArgs e)
        {
            // Make open file picker dialog appear with all audio formats and video formats supported by NAudio
            // Add selected file to the soundboard

            // Open file picker dialog code
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            var hWndCurrentWindow = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hWndCurrentWindow);

            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            fileOpenPicker.CommitButtonText = "Add to Soundboard";
            fileOpenPicker.FileTypeFilter.Add(".mp3");
            fileOpenPicker.FileTypeFilter.Add(".wav");
            fileOpenPicker.FileTypeFilter.Add(".wma");
            fileOpenPicker.FileTypeFilter.Add(".m4a");
            fileOpenPicker.FileTypeFilter.Add(".flac");
            fileOpenPicker.FileTypeFilter.Add(".aac");
            fileOpenPicker.FileTypeFilter.Add(".mp4");
            fileOpenPicker.FileTypeFilter.Add(".wmv");
            fileOpenPicker.FileTypeFilter.Add(".avi");
            fileOpenPicker.FileTypeFilter.Add(".mkv");
            fileOpenPicker.FileTypeFilter.Add(".mov");
            fileOpenPicker.FileTypeFilter.Add(".m4v");

            // Multiselect is supported
            var files = await fileOpenPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                soundBoard.Add(files.ToList());
            }
        }

        private async void DownloadSoundFile_Click(object sender, RoutedEventArgs e)
        {
            // Make download file dialog appear
            // Add downloaded file to the soundboard

            // Open download file dialog code
            DownloadFileDialog downloadFileDialog = new DownloadFileDialog();
            ContentDialogResult result = await MainWindow.g_appMessagingEvents.ShowMessageBox("Download File", "", "Cancel", "Download File", null, ContentDialogButton.Primary, downloadFileDialog);

            if (result == ContentDialogResult.Primary)
            {
                soundBoard.AddInternetAudio(DownloadFileDialog.userSelectedFileUrl, (DownloadFileDialog.overrideFileName && DownloadFileDialog.userSelectedFileName != "") ? DownloadFileDialog.userSelectedFileName : null);

                // TODO: Add download logic.
            }
        }

        private void SoundboardContextFlyoutViewInExplorerBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the soundboard Item that was right clicked
            SoundboardItem item = (SoundboardItem)((FrameworkElement)sender).DataContext;
            if (item != null) {
                System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + item.PhysicalFilePath + "\"");
            }
        }

        private async void AddTTSAudio_Click(object sender, RoutedEventArgs e)
        {
            AddTTSAudioDialog dialog = new AddTTSAudioDialog();
            ContentDialogResult result = await MainWindow.g_appMessagingEvents.ShowMessageBox("Add Text to Speech Audio", "", "Cancel", "Add", null, ContentDialogButton.Primary, dialog);

            if (result == ContentDialogResult.Primary)
            {
                soundBoard.Add(dialog.viewModel.Name, dialog.viewModel.TtsText, dialog.viewModel.SpeedMultiplierValue, dialog.viewModel.SpeedMultiplierValue, false);
            }
        }
    }
}
