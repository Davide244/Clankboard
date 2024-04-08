using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.WinUI.Controls;
using static Clankboard.AudioManager;
using Clankboard.Classes;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Pages
{
    public sealed partial class SettingsPage : Page
    {

        // List of audio output devices
        private List<AudioDevice> AudioOutputDevices { get; set; } = new List<AudioDevice> { };
        private List<AudioDevice> AudioInputDevices { get; set; } = new List<AudioDevice> { };

        private bool SettingsEnabled = false;

        public void UpdateComboboxes()
        {
            AudioOutputDevices = AudioManager.GetAudioOutputDevices();
            AudioInputDevices = AudioManager.GetAudioInputDevices();

            foreach (var device in AudioOutputDevices)
            {
                OutputDeviceCombobox.Items.Add(device.DeviceName);
                DriverInputCombobox.Items.Add(device.DeviceName);
            }

            foreach (var device in AudioInputDevices)
            {
                InputDeviceCombobox.Items.Add(device.DeviceName);
            }

            string CurrentOutputDeviceID = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice).DeviceID;
            string CurrentInputDeviceID = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.InputDevice).DeviceID;
            string CurrentDriverInputDeviceID = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice).DeviceID; // NOTE: This is a logical output device. Called "Input" because it is the input to the virtual audio cable.

            // Set combobox indexes to the current audio devices
            for (int i = 0; i < AudioOutputDevices.Count; i++)
            {
                if (AudioOutputDevices[i].DeviceID == CurrentOutputDeviceID)
                {
                    OutputDeviceCombobox.SelectedIndex = i + 1;
                }
                if (AudioInputDevices[i].DeviceID == CurrentDriverInputDeviceID)
                {
                    DriverInputCombobox.SelectedIndex = i;
                }
            }

            for (int i = 0; i < AudioInputDevices.Count; i++)
            {
                if (AudioInputDevices[i].DeviceID == CurrentInputDeviceID)
                {
                    InputDeviceCombobox.SelectedIndex = i + 1;
                }
            }
        }

        private void UpdateToggleSwitches()
        {
            OutputMixerToggle.IsOn = SettingsManager.GetSetting<bool>(SettingsManager.SettingTypes.OutputMicrophoneToVAC);
            HearYourselfToggle.IsOn = SettingsManager.GetSetting<bool>(SettingsManager.SettingTypes.HearYourselfEnabled);
            StackedAudioToggle.IsOn = SettingsManager.GetSetting<bool>(SettingsManager.SettingTypes.AudioStackingEnabled);
        }

        public SettingsPage()
        {
            this.InitializeComponent();
            UpdateComboboxes();
            UpdateToggleSwitches();

            SettingsEnabled = true;
        }

        // Event that covers the changing of the audio device comboboxes
        private void AudioDeviceCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            if (combobox.SelectedIndex == -1 || SettingsEnabled == false)
            {
                return;
            }

            if (combobox.Name == "OutputDeviceCombobox")
            {
                // Write previous output device to the debug log
                Debug.WriteLine($"Previous Output Device: {SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice).DeviceName}");

                if (combobox.SelectedIndex == 0)
                {
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.LocalOutputDevice, SettingsManager.DefaultAudioDevice);
                }

                if (combobox.SelectedIndex - 1 >= 0 && combobox.SelectedIndex - 1 <= AudioOutputDevices.Count)
                {
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.LocalOutputDevice, new AudioDevice { DeviceName = AudioOutputDevices[combobox.SelectedIndex - 1].DeviceName, DeviceID = AudioOutputDevices[combobox.SelectedIndex - 1].DeviceID });
                    Debug.WriteLine($"New Output Device: {SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice).DeviceName}");
                }
            }
            else if (combobox.Name == "InputDeviceCombobox")
            {
                // Write previous input device to the debug log
                Debug.WriteLine($"Previous Input Device: {SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.InputDevice).DeviceName}");

                if (combobox.SelectedIndex == 0)
                {
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.InputDevice, SettingsManager.DefaultAudioDevice);
                }

                if (combobox.SelectedIndex - 1 >= 0 && combobox.SelectedIndex - 1 <= AudioInputDevices.Count)
                {
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.InputDevice, new AudioDevice { DeviceName = AudioInputDevices[combobox.SelectedIndex - 1].DeviceName, DeviceID = AudioInputDevices[combobox.SelectedIndex - 1].DeviceID });
                    Debug.WriteLine($"New Input Device: {SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.InputDevice).DeviceName}");
                }
            }
            else if (combobox.Name == "DriverInputCombobox")
            {
                // Write previous driver input device to the debug log
                Debug.WriteLine($"Previous Driver Input Device: {SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice).DeviceName}");

                if (combobox.SelectedIndex >= 0 && combobox.SelectedIndex <= AudioOutputDevices.Count)
                {
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.VACOutputDevice, new AudioDevice { DeviceName = AudioOutputDevices[combobox.SelectedIndex].DeviceName, DeviceID = AudioOutputDevices[combobox.SelectedIndex].DeviceID });
                    Debug.WriteLine($"New Driver Input Device: {SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice).DeviceName}");
                }
            }
        }

        private void SettingToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!SettingsEnabled)
                return;

            var toggleSwitch = sender as ToggleSwitch;

            switch(toggleSwitch.Name)
            {
                case "OutputMixerToggle":
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.OutputMicrophoneToVAC, toggleSwitch.IsOn);
                    break;
                case "HearYourselfToggle":
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.HearYourselfEnabled, toggleSwitch.IsOn);
                    break;
                case "StackedAudioToggle":
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.AudioStackingEnabled, toggleSwitch.IsOn);
                    break;
                case "UseCompactSoundboardListToggle":
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.UseCompactSoundboardList, toggleSwitch.IsOn);
                    break;
            }
            //if (toggleSwitch.Name == "OutputMixerToggle")
            //{
            //    SettingsManager.SetSetting(SettingsManager.SettingTypes.OutputMicrophoneToVAC, toggleSwitch.IsOn);
            //}
            //else if (toggleSwitch.Name == "HearYourselfToggle")
            //{
            //    SettingsManager.SetSetting(SettingsManager.SettingTypes.HearYourselfEnabled, toggleSwitch.IsOn);
            //}
        }

        //private void UseDefaultAudioDevicesBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    UpdateComboboxes();
        //    OutputDeviceCombobox.SelectedIndex = 0;
        //    InputDeviceCombobox.SelectedIndex = 0;
        //    DriverInputCombobox.SelectedIndex = -1;
        //}

        //private void ApplyAudioDeviceChangesBtn_Click(object sender, RoutedEventArgs e)
        //{
            
        //}
    }
}
