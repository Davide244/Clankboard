using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Systems
{
    public partial class SettingsSystemViewmodel : ObservableObject
    {
        #region Singleton
        private static readonly SettingsSystemViewmodel instance = new SettingsSystemViewmodel();

        static SettingsSystemViewmodel() 
        {
        }

        public static SettingsSystemViewmodel Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        [ObservableProperty]
        private bool _audioMixingEnabled;
        [ObservableProperty]
        private bool _inputLoopbackEnabled;

        [ObservableProperty]
        private int _inputVolume;
        [ObservableProperty]
        private int _outputVolume;
        [ObservableProperty]
        private int _localOutputVolume;

        public SettingsSystemViewmodel()
        {
            AudioMixingEnabled = true;
            InputLoopbackEnabled = false;

            InputVolume = 100;
            OutputVolume = 100;
            LocalOutputVolume = 100;
        }
    }
}
