using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Systems
{
    public abstract class LoadedClankFile 
    {
        private string Path;

        public LoadedClankFile(string path) 
        {
            Path = path;
        }
    }

    public struct SettingsFile
    {
        public bool AudioMixingEnabled { get; set; }
        public bool InputLoopbackEnabled { get; set; }
        public bool MicrophoneMuted { get; set; }
        public int InputVolume { get; set; }
        public int OutputVolume { get; set; }
        public int LocalOutputVolume { get; set; }
        public bool GridViewInSoundboardEnabled { get; set; }

        public bool SkipYTDLPDownloadConfirmationDialog { get; set; }
        public bool SkipFFMPEGDownloadConfirmationDialog { get; set; }
        public bool SkipFFPROBEDownloadConfirmationDialog { get; set; }

        public int SelectedOutputDeviceIndex { get; set; }
        public int SelectedLocalOutputDeviceIndex { get; set; }
        public int SelectedInputDeviceIndex { get; set; }
    }

    public class SoundboardFile : LoadedClankFile
    {
        public SoundboardFile(string name, string path) : base(path) 
        {

        }
    }


}
