using Clankboard.AudioSystem;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

            // Add random soundboard items for testing
            soundBoard.Add("Test 1", "C:\\Users\\Public\\Music\\Sample Music\\Kalimba.mp3");
            soundBoard.Add("Test 2", "C:\\Users\\Public\\Music\\Sample Music\\Kalimba.mp3");
            soundBoard.Add("Test 3", "C:\\Users\\Public\\Music\\Sample Music\\Kalimba.mp3");
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
                soundBoard.AddLocalAudio(files.ToList());
            }
        }

        private void DownloadSoundFile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
