using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Windows.Storage.Pickers;
using Clankboard.AudioSystem;
using Clankboard.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;
using Clankboard.Services.TTS;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Views.Pages.Soundboard;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SoundboardPageView : Page
{
    private readonly ITTSService ttsService;

    //private readonly Soundboard soundBoard = new();
    private SoundboardPageViewModel viewModel = new();

    public SoundboardPageView(ITTSService ttsService)
    {
        this.ttsService = ttsService;
        InitializeComponent();
    }

    private async void AddLocalSoundFile_Click(object sender, RoutedEventArgs e)
    {
        // Make open file picker dialog appear with all audio formats and video formats supported by NAudio
        // Add selected file to the soundboard

        // Open file picker dialog code
        var fileOpenPicker = new FileOpenPicker();
        var hWndCurrentWindow = WindowNative.GetWindowHandle(App.m_window);
        InitializeWithWindow.Initialize(fileOpenPicker, hWndCurrentWindow);

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
        //if (files.Count > 0) viewModel.SoundboardItems.Add(files.ToList());

        //temp NotImplemented messagebox

    }

    private async void DownloadSoundFile_Click(object sender, RoutedEventArgs e)
    {
        // Make download file dialog appear
        // Add downloaded file to the soundboard

        // Open download file dialog code
        var downloadFileDialog = new DownloadFileDialog();
        var result = await MainWindow.g_appMessagingEvents.ShowMessageBox("Download File", "", "Cancel",
            "Download File", null, ContentDialogButton.Primary, downloadFileDialog);

        //if (result == ContentDialogResult.Primary)
        //    soundBoard.AddInternetAudio(DownloadFileDialog.userSelectedFileUrl,
        //        DownloadFileDialog.overrideFileName && DownloadFileDialog.userSelectedFileName != ""
        //            ? DownloadFileDialog.userSelectedFileName
        //            : null);
        // TODO: Add download logic.
    }

    private void SoundboardContextFlyoutViewInExplorerBtn_Click(object sender, RoutedEventArgs e)
    {
        // Get the soundboard Item that was right clicked
        var item = (SoundboardItem)((FrameworkElement)sender).DataContext;
        if (item != null) Process.Start("explorer.exe", "/select, \"" + item.PhysicalFilePath + "\"");
    }

    private async void AddTTSAudio_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddTTSAudioDialog(ttsService);
        var result = await MainWindow.g_appMessagingEvents.ShowMessageBox("Add Text to Speech Audio", "", "Cancel",
            "Add", null, ContentDialogButton.Primary, dialog); 

        //if (result == ContentDialogResult.Primary)
        //    soundBoard.Add(dialog.viewModel.Name, dialog.viewModel.TtsText, dialog.viewModel.SpeedMultiplierValue,
        //        dialog.viewModel.SpeedMultiplierValue, false);
    }
}