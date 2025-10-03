using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Systems.Settings
{
    public partial class AppSettingsModel : ObservableObject
    {
        [ObservableProperty]
        public partial AppSettings _loadedSettings { get; set; }

#nullable enable
        public AppSettingsModel(string? settingsFilePath)
        {
            if (string.IsNullOrEmpty(settingsFilePath))
            {
                LoadedSettings = new AppSettings();
            }
        }
#nullable disable
    }
}
