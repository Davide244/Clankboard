using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Converter;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Clankboard;

public partial class SoundBoardItem : ObservableObject
{
    [ObservableProperty]
    public string _soundName;
    [ObservableProperty]
    public string _soundLocation;
    [ObservableProperty]
    public string _soundLocationIcon;
    [ObservableProperty]
    public string _soundIconColor;
    [ObservableProperty]
    public string _soundIconVisible;
    [ObservableProperty]
    public string _soundIconTooltip;

    [ObservableProperty]
    public string _soundKeybindForecolor;
    [ObservableProperty]
    public string _soundKeybind;
    [ObservableProperty]
    public bool _progressRingEnabled;
    [ObservableProperty]
    public bool _progressRingIntermediate;
    [ObservableProperty]
    public int _progressRingProgress;
    [ObservableProperty]
    public bool _btnEnabled;
    [ObservableProperty]
    public bool _deleteBtnEnabled;

    // Non-Observable fields
    public string PhysicalFilePath { get; set; }

    public SoundBoardItem(string soundName, string soundLocation, string soundLocationIcon, bool soundIconVisible, string soundIconTooltip, string soundKeybind, bool progressRingEnabled, bool btnEnabled, bool progressRingIntermediate = true, int progressRingProgress = 100, string soundIconColor = null, string soundKeybindForecolor = null, string physicalFilePath = null, bool deleteBtnEnabled = true)
    {

        SoundName = soundName;
        SoundLocation = soundLocation;
        SoundLocationIcon = soundLocationIcon;
        SoundIconColor = soundIconColor;
        SoundIconTooltip = soundIconTooltip;
        SoundKeybindForecolor = soundKeybindForecolor;
        SoundKeybind = soundKeybind;
        ProgressRingEnabled = progressRingEnabled;
        ProgressRingIntermediate = progressRingIntermediate;
        ProgressRingProgress = progressRingProgress;
        BtnEnabled = btnEnabled;
        DeleteBtnEnabled = deleteBtnEnabled;
        if (soundIconVisible == true) SoundIconVisible = "Visible"; else SoundIconVisible = "Collapsed"; // ease of use

        PhysicalFilePath = physicalFilePath;
    }
}

public partial class SoundBoardItemViewmodel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<SoundBoardItem> soundBoardItems = new();
}

public sealed partial class SoundboardPage : Page
{
    public static SoundboardEvents g_SoundboardEvents = new();
    public static SoundBoardItemViewmodel soundBoardItemViewmodel = new();
    const string LocalFileIcon = "\uE8A5";
    const string WarningFileIcon = "\uE783";
    const string DownloadedFileIcon = "\uE753";
    const string DownloadingFileIcon = "\uEBD3";

    private bool SoundboardOptionsMinimized = false;

    private int OngoingDownloads = 0;

    public SoundboardPage()
    {
        this.InitializeComponent();
        
        // Events
        g_SoundboardEvents.NewSoundboardItem_FILE += AddSoundFile;
        g_SoundboardEvents.NewSoundboardItem_URL += DownloadSoundFile;
        g_SoundboardEvents.DeleteAllSoundboardItems += RemoveAllSounds;
        soundBoardItemViewmodel.SoundBoardItems.CollectionChanged += SoundBoardItemsContentChanged;

        MainSoundboardListview.ItemsSource = soundBoardItemViewmodel.SoundBoardItems;
    }

    private void LocalSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        ShellPage.g_ShellPageEvents.OpenAppSettings(sender);
    }

    private void AddSoundFile(object sender, RoutedEventArgs e, string Name, string FilePath)
    {
        //if (!Regex.IsMatch(FilePath, "^.*\\.(mp3|.ogg|.wav|.mp4)$", RegexOptions.IgnoreCase)) return; // To self: broken & wont fix :(
        if (File.Exists(FilePath))
        {
            if (soundBoardItemViewmodel.SoundBoardItems.Any(x => x.SoundLocation == FilePath))
            {
                ShellPage.g_AppMessageBox.ShowMessagebox("File already exists", "The specified file already exists in this soundboard.\nThe file has not been added.", "", "", "Okay", ContentDialogButton.Close);
                return;
            }

            soundBoardItemViewmodel.SoundBoardItems.Add(new SoundBoardItem(Name, FilePath, LocalFileIcon, true, "Local File", "None", false, true, true, 0, null, null, FilePath));
        }
        else
            ShellPage.g_AppMessageBox.ShowMessagebox("File not found", "The specified file could not be found!\nPlease check if the file exists and try again.", "", "", "Okay", ContentDialogButton.Close);
    }

    private void RemoveAllSounds(object Sender, EventArgs e)
    {
        // loop through all items and remove them
        soundBoardItemViewmodel.SoundBoardItems.ToList().ForEach(item =>
        {
            if (item.PhysicalFilePath != null)
            {
                if (item.PhysicalFilePath.Contains($"{AppDomain.CurrentDomain.BaseDirectory}DownloadedSounds\\") && File.Exists(item.PhysicalFilePath))
                    File.Delete(item.PhysicalFilePath);
            }
        });

        soundBoardItemViewmodel.SoundBoardItems.Clear();
    }

    // TODO: FIX MEMORY LEAKS IN THIS FUNCTION. MEMORY DOES NOT GET FREED AFTER FINISHING DOWNLOADS !!!! URGENT !!!!!
    private async void DownloadSoundFile(object sender, RoutedEventArgs e, string Name, string Url)
    {
        if (soundBoardItemViewmodel.SoundBoardItems.Any(x => x.SoundLocation == Url || x.SoundLocation == $"Downloading {Url}"))
        {
            await ShellPage.g_AppMessageBox.ShowMessagebox("URL already exists", "The specified URL / Sound already exist in this soundboard.\nThe sound has not been added.", "", "", "Okay", ContentDialogButton.Close);
            return;
        }

        OngoingDownloads++;
        ShellPage.g_AppInfobar.OpenAppInfobar(AppInfobar.AppInfobarType.FileDownloadInfobar);

        YoutubeClient youtube = new();

        StreamManifest streamManifest = null;
        IStreamInfo streamInfo;
        Stream audioOnlyStreamInfo = null;
        string CurrentName;

        string TempDeleteFilePath = "";

        var FilePath = $"{AppDomain.CurrentDomain.BaseDirectory}DownloadedSounds\\";

        if (!Directory.Exists(FilePath))
            Directory.CreateDirectory(FilePath);

        try
        {
            streamManifest = await youtube.Videos.Streams.GetManifestAsync(Url);
            streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            audioOnlyStreamInfo = await youtube.Videos.Streams.GetAsync(streamInfo);

            Video VideoInfo = (await youtube.Videos.GetAsync(Url));

            if (Name != "")
                CurrentName = Name;
            else
                CurrentName = VideoInfo.Title;

            SoundBoardItem soundboardItem = new(CurrentName, $"Downloading {Url}", DownloadedFileIcon, false, "Downloaded File", "None", true, false, false, 0);

            CancellationTokenSource cts = new CancellationTokenSource();
            soundBoardItemViewmodel.SoundBoardItems.Add(soundboardItem);

            var ProgressHandler = new Progress<double>(p =>
            {
                if (soundBoardItemViewmodel.SoundBoardItems.All(x => x.SoundLocation != Url && x.SoundLocation != $"Downloading {Url}"))
                {
                    TempDeleteFilePath = FilePath + $"audio_youtube.{VideoInfo.Id}.{streamInfo.Container}";
                    cts.Cancel();
                }

                soundboardItem.ProgressRingProgress = Convert.ToInt32(p * 100);
            });

            await youtube.Videos.Streams.DownloadAsync(streamInfo, FilePath + $"audio_youtube.{VideoInfo.Id}.{streamInfo.Container}", ProgressHandler, cts.Token);
            soundboardItem.ProgressRingIntermediate = true;

            soundboardItem.PhysicalFilePath = FilePath + $"audio_youtube.{VideoInfo.Id}.{streamInfo.Container}";
            soundboardItem.ProgressRingIntermediate = true;
            soundboardItem.ProgressRingEnabled = false;
            soundboardItem.BtnEnabled = true;
            soundboardItem.SoundIconVisible = "Visible";
            soundboardItem.SoundLocation = Url;
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                if (File.Exists(TempDeleteFilePath))
                {
                    File.Delete(TempDeleteFilePath);
                }
            }
            else
            {
                OngoingDownloads--;
                if (OngoingDownloads <= 0) ShellPage.g_AppInfobar.OpenAppInfobar(AppInfobar.AppInfobarType.FileDownloadInfobar, false);
                await ShellPage.g_AppMessageBox.ShowMessagebox("Download Error", $"An error has occured while downloading the youtube video: {Url}\n\nPlease make sure that the URL is correct and links to a valid youtube video.\nPlease note that age restricted videos are not supported.\n\nException: {ex.Message}", "", "", "Okay", ContentDialogButton.Close);
                return;
            }
        }

        OngoingDownloads--;
        if (OngoingDownloads <= 0) ShellPage.g_AppInfobar.OpenAppInfobar(AppInfobar.AppInfobarType.FileDownloadInfobar, false);

        // Cleanup
        audioOnlyStreamInfo?.Dispose();
        GC.Collect();
    }

    // Soundboard button Events

    private void Soundboard_Removeitem_Click(object sender, RoutedEventArgs e)
    {
        var item = ((FrameworkElement)sender).DataContext;
        var index = MainSoundboardListview.Items.IndexOf(item);
        
        if (soundBoardItemViewmodel.SoundBoardItems[index].PhysicalFilePath != null && soundBoardItemViewmodel.SoundBoardItems[index].PhysicalFilePath.Contains($"{AppDomain.CurrentDomain.BaseDirectory}DownloadedSounds\\") && File.Exists(soundBoardItemViewmodel.SoundBoardItems[index].PhysicalFilePath))
            File.Delete(soundBoardItemViewmodel.SoundBoardItems[index].PhysicalFilePath);

        soundBoardItemViewmodel.SoundBoardItems.RemoveAt(index);

        GC.Collect();
    }

    private void SoundboardOptions_Minimize(bool Minimized)
    {
        if (Minimized)
        {
            SoundboardOptions_Expanded.Visibility = Visibility.Collapsed;
            SoundboardOptions_MinimizeBtn.Content = "\uFF0B";
        }
        else
        {
            SoundboardOptions_Expanded.Visibility = Visibility.Visible;
            SoundboardOptions_MinimizeBtn.Content = "\u2015";
        }
    }

    private void SoundboardOptions_MinimizeOptions_Click(object sender, RoutedEventArgs e)
    {
        if (SoundboardOptionsMinimized)
        {
            SoundboardOptions_Minimize(false);
            SoundboardOptionsMinimized = false;
        }
        else
        {
            SoundboardOptions_Minimize(true);
            SoundboardOptionsMinimized = true;
        }
    }

    private void MainSoundboardListview_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (soundBoardItemViewmodel.SoundBoardItems.Count > 0)
        {
            SoundboardNoItems.Visibility = Visibility.Collapsed;
            MainSoundboardListViewScroll.Visibility = Visibility.Visible;
        }
        else
        {
            SoundboardNoItems.Visibility = Visibility.Visible;
            MainSoundboardListViewScroll.Visibility = Visibility.Collapsed;
        }
    }

    private void SoundBoardItemsContentChanged(object sender, NotifyCollectionChangedEventArgs e) => MainSoundboardListview_ContainerContentChanging(null, null);
}
