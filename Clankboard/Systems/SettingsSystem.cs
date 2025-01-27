using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

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
        private bool _microphoneMuted;

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
            MicrophoneMuted = false;

            InputVolume = 100;
            OutputVolume = 100;
            LocalOutputVolume = 100;

            SaveSettingsFile(Path.Combine(App.appDataFolderManager.GetAppDataFolder(), "settings.json"));
        }

        private void LoadSettingsFile(string path) 
        {
            // Load the JSON file and set the fields in this class
            
        }

        private void SaveSettingsFile(string path) 
        {
            // Save this class data to a JSON file. path is the file location.

            // Serialize SettingsSystemViewmodel
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            // Write to file
            if (!File.Exists(path))
                File.Create(path).Dispose();

            File.WriteAllText(path, json);
        }
    }
}
