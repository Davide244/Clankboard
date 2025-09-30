using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Systems.Settings
{


    /// <summary>
    /// Manages application settings.
    /// </summary>
    public class AppSettings
    {
        #region Boolean Settings
        // Audio related settings
        public bool AudioMixingEnabled { get; set; } = true;
        public bool InputLoopbackEnabled { get; set; } = false;
        public bool MicrophoneMuted { get; set; } = false;

        public bool SkipFFMPEGDownloadConfirmationDialog { get; set; } = false;
        public bool SkipFFPROBEDownloadConfirmationDialog { get; set; } = false;
        #endregion

        public int InputVolume { get; set; } = 100;
        public int LocalOutputVolume { get; set; } = 100;
        public int OutputVolume { get; set; } = 100;

        public int SelectedInputDeviceIndex { get; set; } = -1;
        public int SelectedLocalOutputDeviceIndex { get; set; } = -1;
        public int SelectedOutputDeviceIndex { get; set; } = -1;

        // Confirmation Dialog Skips
        public bool SkipYTDLPDownloadConfirmationDialog { get; set; } = true;


    }
}
