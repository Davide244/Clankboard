using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SoundboardItem(string itemName, string itemLocation, SoundboardItemType itemType, string physicalFilePath, bool interactionEnabled = true, bool itemLoadingIndicatorsVisible = false)
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

            ItemErrorIndicatorVisibility = "Collapsed";

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

        public void AddLocalAudio(List<Windows.Storage.StorageFile> files)
        {
            foreach (var file in files)
            {
                SoundboardItem item = new SoundboardItem(file.DisplayName, file.Path, SoundboardItemType.LocalFile, file.Path);
                soundboardViewmodel.SoundboardItems.Add(item);
            }
        }

        public void AddLocalAudio(string filePath)
        {
            SoundboardItem soundboardItem = new SoundboardItem("Test", "Test", SoundboardItemType.LocalFile, filePath);
            soundboardViewmodel.SoundboardItems.Add(soundboardItem);
        }

        public void Add(string name, string pathText)
        {
            SoundboardItem item = new SoundboardItem(name, pathText, SoundboardItemType.LocalFile, pathText);
            soundboardViewmodel.SoundboardItems.Add(item);
        }
    }
}
