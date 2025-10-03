using Clankboard.AudioSystem;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Views.Pages.Soundboard
{
    public partial class SoundboardListItem : ObservableObject
    {
        private const string LocalFileIcon = "\uE8A5";
        private const string WarningFileIcon = "\uE783";
        private const string DownloadedFileIcon = "\uE753";
        private const string TTSFileIcon = "\uF2B7";

        [ObservableProperty] private bool _canClickConfigure;
        [ObservableProperty] private bool _canClickExport;
        [ObservableProperty] private bool _canClickViewInExplorer;
        [ObservableProperty] private bool _isPlayButtonEnabled;
        [ObservableProperty] private string _itemErrorIndicatorVisibility;
        [ObservableProperty] private string _itemIcon;
        // [ObservableProperty] private string _itemIconColor; // Removed
        [ObservableProperty] private string _itemIconVisibility;
        [ObservableProperty] private string _itemKeybindText;
        [ObservableProperty] private string _itemLoadingIndicatorsVisibility;
        [ObservableProperty] private string _itemLocationText;
        [ObservableProperty] private string _itemName;
        [ObservableProperty] private int _itemProgressRingProgress;
        [ObservableProperty] private bool _userDeletionEnabled;

        public string PhysicalFilePath;
        public SoundboardItemType ItemType { get; private set; }

        public SoundboardListItem(string itemName, string itemLocation, SoundboardItemType itemType, string physicalFilePath,
            bool interactionEnabled = true, bool itemLoadingIndicatorsVisible = false,
            bool itemErrorIndicatorsVisible = false)
        {
            ItemName = itemName;
            ItemLocationText = itemLocation;
            ItemType = itemType;
            PhysicalFilePath = physicalFilePath;

            ItemKeybindText = "Ctrl + E";

            switch (itemType)
            {
                case SoundboardItemType.LocalFile:
                    _itemIcon = LocalFileIcon;
                    break;
                case SoundboardItemType.DownloadedFile:
                    _itemIcon = DownloadedFileIcon;
                    break;
                case SoundboardItemType.TTSFile:
                    _itemIcon = TTSFileIcon;
                    break;
            }

            SetInteractionEnabled(interactionEnabled);
            SetErrorIndicatorVisibility(itemErrorIndicatorsVisible);
            SetProgressIndicatorVisibility(itemLoadingIndicatorsVisible);
        }

        public void SetProgressIndicatorVisibility(bool visible)
        {
            ItemLoadingIndicatorsVisibility = visible ? "Visible" : "Collapsed";
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
}
