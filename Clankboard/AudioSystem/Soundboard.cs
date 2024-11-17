using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.AudioSystem
{

    public partial class SoundboardItem : ObservableObject
    {
        [ObservableProperty]
        public string _name;
        [ObservableProperty]
        public string _locationText;

        public string PhysicalFilePath;
    }

    class Soundboard
    {
        public const string LocalFileIcon = "\uE8A5";
        public const string WarningFileIcon = "\uE783";
        public const string DownloadedFileIcon = "\uE753";
        public const string TTSFileIcon = "\uF2B7";

        private static int activeDownloads = 0;
    }
}
