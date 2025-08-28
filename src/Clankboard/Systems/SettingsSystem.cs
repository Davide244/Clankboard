using System;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;

namespace Clankboard.Systems;

public partial class SettingsSystemViewmodel : ObservableObject
{
    [ObservableProperty] private bool _audioMixingEnabled;

    [ObservableProperty] private bool _gridViewInSoundboardEnabled;

    [ObservableProperty] private bool _inputLoopbackEnabled;

    [ObservableProperty] private int _inputVolume;

    [ObservableProperty] private int _localOutputVolume;

    [ObservableProperty] private bool _microphoneMuted;

    [ObservableProperty] private int _outputVolume;

    [ObservableProperty] public int _selectedInputDeviceIndex;

    [ObservableProperty] public int _selectedLocalOutputDeviceIndex;

    [ObservableProperty] public int _selectedOutputDeviceIndex;

    [ObservableProperty] private bool _skipFFMPEGDownloadConfirmationDialog;

    [ObservableProperty] private bool _skipFFPROBEDownloadConfirmationDialog;

    // Confirmation Dialog Skips
    [ObservableProperty] private bool _skipYTDLPDownloadConfirmationDialog;

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
            Debug.WriteLine(
                "Could not load settings.json file from AppData. Fell back to defaults. Exception Message: " +
                e.Message);
        }
    }

    private void LoadSettingsFile(string path)
    {
        // Load the JSON file and set the fields in this class

        // Serialize the JSON file
        var json = File.ReadAllText(path);

        // Deserialize the JSON file
        var settings = JsonConvert.DeserializeObject<SettingsFile>(json);

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

    public void Save()
    {
        SaveSettingsFile(Path.Combine(App.appDataFolderManager.GetAppDataFolder(), "settings.json"));
    }

    private void SaveSettingsFile(string path)
    {
        // Save the fields in this class to a JSON file
        var settings = new SettingsFile();

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
        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);

        // Write the JSON to the file
        try
        {
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.WriteLine("Could not save settings.json file to AppData. Exception Message: " + e.Message);
            // Display retry dialog to user.
            var result = MainWindow.g_appMessagingEvents.ShowMessageBox("Error Saving Settings",
                "An error occured while writing to the settings.json file. Settings have not been saved.", "Okay",
                "Retry", null, ContentDialogButton.Primary).Result;
            if (result == ContentDialogResult.Primary) Save(); // Retry the save procedure.
        }
    }

    #region Singleton

    static SettingsSystemViewmodel()
    {
    }

    public static SettingsSystemViewmodel Instance { get; } = new();

    #endregion
}