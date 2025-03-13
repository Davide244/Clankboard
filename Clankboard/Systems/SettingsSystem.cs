using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Clankboard.Utils;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;

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
        [ObservableProperty]
        private bool _gridViewInSoundboardEnabled;

        // Confirmation Dialog Skips
        [ObservableProperty]
        private bool _skipYTDLPDownloadConfirmationDialog;
        [ObservableProperty]
        private bool _skipFFMPEGDownloadConfirmationDialog;
        [ObservableProperty]
        private bool _skipFFPROBEDownloadConfirmationDialog;

        [ObservableProperty]
        public int _selectedOutputDeviceIndex;
        [ObservableProperty]
        public int _selectedLocalOutputDeviceIndex;
        [ObservableProperty]
        public int _selectedInputDeviceIndex;

        public SettingsSystemViewmodel()
        {
            AudioMixingEnabled = true;
            InputLoopbackEnabled = false;
            MicrophoneMuted = false;

            InputVolume = 100;
            OutputVolume = 100;
            LocalOutputVolume = 100;

            GridViewInSoundboardEnabled = false;

            SkipFFMPEGDownloadConfirmationDialog = false;
            SkipFFPROBEDownloadConfirmationDialog = false;
            SkipYTDLPDownloadConfirmationDialog = false;

            try
            {
                LoadSettingsFile(Path.Combine(App.appDataFolderManager.GetAppDataFolder(), "settings.json"));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not load settings.json file from AppData. Fell back to defaults. Exception Message: " + e.Message);
            }
        }

        private void LoadSettingsFile(string path) 
        {
            // Load the JSON file and set the fields in this class

            // Serialize the JSON file
            string json = File.ReadAllText(path);

            // Deserialize the JSON file
            SettingsFile settings = JsonConvert.DeserializeObject<SettingsFile>(json);

            // Set the fields
            AudioMixingEnabled = settings.AudioMixingEnabled;
            InputLoopbackEnabled = settings.InputLoopbackEnabled;
            MicrophoneMuted = settings.MicrophoneMuted;
            InputVolume = settings.InputVolume;
            OutputVolume = settings.OutputVolume;
            LocalOutputVolume = settings.LocalOutputVolume;
            GridViewInSoundboardEnabled = settings.GridViewInSoundboardEnabled;

            SkipYTDLPDownloadConfirmationDialog = settings.SkipYTDLPDownloadConfirmationDialog;
            SkipFFMPEGDownloadConfirmationDialog = settings.SkipFFMPEGDownloadConfirmationDialog;
            SkipFFPROBEDownloadConfirmationDialog = settings.SkipFFPROBEDownloadConfirmationDialog;
        }

        public void Save() => SaveSettingsFile(Path.Combine(App.appDataFolderManager.GetAppDataFolder(), "settings.json"));

        private void SaveSettingsFile(string path) 
        {
            // Save the fields in this class to a JSON file
            SettingsFile settings = new SettingsFile();

            settings.AudioMixingEnabled = AudioMixingEnabled;
            settings.InputLoopbackEnabled = InputLoopbackEnabled;
            settings.MicrophoneMuted = MicrophoneMuted;
            settings.InputVolume = InputVolume;
            settings.OutputVolume = OutputVolume;
            settings.LocalOutputVolume = LocalOutputVolume;
            settings.GridViewInSoundboardEnabled = GridViewInSoundboardEnabled;

            settings.SkipYTDLPDownloadConfirmationDialog = SkipYTDLPDownloadConfirmationDialog;
            settings.SkipFFMPEGDownloadConfirmationDialog = SkipFFMPEGDownloadConfirmationDialog;
            settings.SkipFFPROBEDownloadConfirmationDialog = SkipFFPROBEDownloadConfirmationDialog;

            settings.SelectedOutputDeviceIndex = SelectedOutputDeviceIndex;
            settings.SelectedLocalOutputDeviceIndex = SelectedLocalOutputDeviceIndex;
            settings.SelectedInputDeviceIndex = SelectedInputDeviceIndex;

            // Serialize the settings
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);

            // Write the JSON to the file
            try 
            {
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not save settings.json file to AppData. Exception Message: " + e.Message);
                // Display retry dialog to user.
                ContentDialogResult result = MainWindow.g_appMessagingEvents.ShowMessageBox("Error Saving Settings", "An error occured while writing to the settings.json file. Settings have not been saved.", "Okay", "Retry", null, ContentDialogButton.Primary).Result;
                if (result == ContentDialogResult.Primary) 
                {
                    Save(); // Retry the save procedure.
                }
            }
        }
    }
}
