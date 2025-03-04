using Clankboard.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using YoutubeDLSharp;
using YoutubeDLSharp.Converters;
using YoutubeDLSharp.Metadata;

namespace Clankboard.AudioSystem
{
    public enum SoundboardItemType
    {
        LocalFile,
        DownloadedFile,
        TTSFile
    }

    public partial class SoundboardItem : ObservableObject
    {
        [ObservableProperty]
        public string _itemName;
        [ObservableProperty]
        public string _itemLocationText;
        [ObservableProperty]
        public string _itemIcon;
        [ObservableProperty]
        public string _itemIconColor;
        [ObservableProperty]
        public string _itemIconVisibility;
        [ObservableProperty]
        public string _itemKeybindText;

        public SoundboardItemType ItemType { get; private set; }
        [ObservableProperty]
        public string _itemLoadingIndicatorsVisibility;
        [ObservableProperty]
        public int _itemProgressRingProgress;
        [ObservableProperty]
        public string _itemErrorIndicatorVisibility;

        [ObservableProperty]
        public bool _isPlayButtonEnabled;
        [ObservableProperty]
        public bool _userDeletionEnabled;
        [ObservableProperty]
        public bool _canClickViewInExplorer;
        [ObservableProperty]
        public bool _canClickConfigure;
        [ObservableProperty]
        public bool _canClickExport;


        public string PhysicalFilePath;

        public SoundboardItem(string itemName, string itemLocation, SoundboardItemType itemType, string physicalFilePath, bool interactionEnabled = true, bool itemLoadingIndicatorsVisible = false, bool itemErrorIndicatorsVisible = false)
        {
            ItemName = itemName;
            ItemLocationText = itemLocation;
            ItemType = itemType;
            PhysicalFilePath = physicalFilePath;

            ItemKeybindText = "Ctrl + E";

            switch (itemType)
            {
                case SoundboardItemType.LocalFile:
                    _itemIcon = Soundboard.LocalFileIcon;
                    break;
                case SoundboardItemType.DownloadedFile:
                    _itemIcon = Soundboard.DownloadedFileIcon;
                    break;
                case SoundboardItemType.TTSFile:
                    _itemIcon = Soundboard.TTSFileIcon;
                    break;
            }

            IsPlayButtonEnabled = interactionEnabled;
            UserDeletionEnabled = interactionEnabled;
            CanClickViewInExplorer = interactionEnabled;
            CanClickConfigure = interactionEnabled;
            CanClickExport = interactionEnabled;

            ItemErrorIndicatorVisibility = itemErrorIndicatorsVisible ? "Visible" : "Collapsed";

            ItemIconColor = "White";

            ItemLoadingIndicatorsVisibility = itemLoadingIndicatorsVisible ? "Visible" : "Collapsed";
            // Hide icon when indicators are visible
            ItemIconVisibility = !itemLoadingIndicatorsVisible ? "Visible" : "Collapsed";
        }

        public void SetProgressIndicatorVisibility(bool visible)
        {
            ItemLoadingIndicatorsVisibility = visible ? "Visible" : "Collapsed";
            // Hide icon when indicators are visible
            ItemIconVisibility = visible ? "Collapsed" : "Visible";
        }

        public void SetErrorIndicatorVisibility(bool visible)
        {
            ItemErrorIndicatorVisibility = visible ? "Visible" : "Collapsed";
        }

        public void SetInteractionEnabled(bool enabled)
        {
            IsPlayButtonEnabled = enabled;
            UserDeletionEnabled = enabled;
            CanClickViewInExplorer = enabled;
            CanClickConfigure = enabled;
            CanClickExport = enabled;
        }
    }

    // Viewmodel
    public partial class SoundboardViewmodel : ObservableObject 
    {
        [ObservableProperty]
        public ObservableCollection<SoundboardItem> _soundboardItems = new ObservableCollection<SoundboardItem>();
    }

    class Soundboard
    {
        public SoundboardViewmodel soundboardViewmodel = new SoundboardViewmodel();

        public const string LocalFileIcon = "\uE8A5";
        public const string WarningFileIcon = "\uE783";
        public const string DownloadedFileIcon = "\uE753";
        public const string TTSFileIcon = "\uF2B7";

        private static int activeDownloads = 0;

        public void Add(List<Windows.Storage.StorageFile> files)
        {
            foreach (var file in files)
            {
                SoundboardItem item = new SoundboardItem(file.DisplayName, file.Path, SoundboardItemType.LocalFile, file.Path);
                soundboardViewmodel.SoundboardItems.Add(item);
            }
        }

        public void Add(string filePath)
        {
            SoundboardItem soundboardItem = new SoundboardItem("Test", "Test", SoundboardItemType.LocalFile, filePath);
            soundboardViewmodel.SoundboardItems.Add(soundboardItem);
        }

        public void Add(SoundboardItem item) => soundboardViewmodel.SoundboardItems.Add(item);


        public async void AddInternetAudio(string fileUrl, string customSoundName = null, bool embedInFile = false) 
        {
            // Check if the file leads to media directly (Aka not a youtube link for example). Check this by seeing if the server prompts a download
            // If it does, download the file to the downloads folder and add it to the soundboard
            // Otherwise, use ytdlp to attempt the download.

            // Add the file to the soundboard
            SoundboardItem item = new SoundboardItem(customSoundName != null ? customSoundName : fileUrl, fileUrl, SoundboardItemType.DownloadedFile, null, false, true);
            soundboardViewmodel.SoundboardItems.Add(item);

            // Check if link exists:
            if (Utils.InetHelper.ValidateUrlWithHttp(fileUrl).Result == false) 
            {
                soundboardViewmodel.SoundboardItems.Remove(item);
                return;
            }

            // Download the file
            var ytdlp = new YoutubeDL();
            //ytdlp.YoutubeDLPath = MainWindow.g_auxSoftwareMgr.YTDLPPath;
            //ytdlp.FFmpegPath = MainWindow.g_auxSoftwareMgr.FFmpegPath;
            ytdlp.OutputFolder = Path.Combine(App.AppDataPath, "Downloads");

            var fetchResult = await ytdlp.RunVideoDataFetch(fileUrl);
            VideoData videoData = fetchResult.Data;
            if (customSoundName == null) item.ItemName = videoData.Title;

            // Check if (ID).wav exists in the downloads folder
            string fileExtension = ".wav";
            string fileName = videoData.DisplayID + fileExtension;
            string filePath = Path.Combine(App.AppDataPath, "Downloads", fileName);

            if (File.Exists(filePath))
            {
                // File exists, add it to the soundboard
                item.PhysicalFilePath = filePath;
                item.SetProgressIndicatorVisibility(false);
                item.SetInteractionEnabled(true);
                return;
            }

            // Set OutputFileTemplate to ID.EXT
            ytdlp.OutputFileTemplate = "%(id)s.%(ext)s";

            var progress = new Progress<DownloadProgress>(p => item.ItemProgressRingProgress = (int)p.Progress*100);

            var result = await ytdlp.RunAudioDownload(fileUrl, YoutubeDLSharp.Options.AudioConversionFormat.Wav, progress: progress);

            string path = result.Data.ToString();
            if (result.Success)
            {
                item.PhysicalFilePath = path;
                item.SetProgressIndicatorVisibility(false);
                item.SetInteractionEnabled(true);
            }
            else
            {
                // Remove the item from the soundboard
                soundboardViewmodel.SoundboardItems.Remove(item);
                Debug.WriteLine("***** DOWNLOAD FAILED! : " + result.ErrorOutput);
            }
        }

        public async void Add(string itemName, string TTSText, int speed, int volume, bool embedded)
        {
            // Add the file to the soundboard
            SoundboardItem item = new SoundboardItem(itemName, "„" + TTSText + "”", SoundboardItemType.TTSFile, null, false, false);

            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            string fileName = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(TTSText))) + ".wav";
            string filePath = Path.Combine(App.AppDataPath, fileName);

            try
            {
                using (var stream = new InMemoryRandomAccessStream())
                {
                    synthesizer.Options.SpeakingRate = speed;
                    var speechStream = await synthesizer.SynthesizeTextToStreamAsync(TTSText);

                    using (var fileStream = File.Create(filePath))
                    {
                        speechStream.AsStreamForRead().CopyTo(fileStream);
                    }
                }

                item.PhysicalFilePath = filePath;
                item.SetInteractionEnabled(true);
                soundboardViewmodel.SoundboardItems.Add(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Schreiben der Datei: {ex.Message}");
            }
        }
    }
}
