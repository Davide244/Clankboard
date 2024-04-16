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
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Pages
{
    public partial class AudioDeviceComboboxItem : ObservableObject
    {
        [ObservableProperty]
        private string _deviceName;
        [ObservableProperty]
        private Guid _deviceGUID;
        [ObservableProperty]
        private int _deviceNumber;

        public AudioDeviceComboboxItem(string deviceName, Guid deviceGUID, int deviceNumber)
        {
            DeviceName = deviceName;
            DeviceGUID = deviceGUID;
            DeviceNumber = deviceNumber;
        }
    }

    // Audio output device is just like the input device, but with a different name. So we can inherit from the input device class
    public partial class AudioOutputDeviceComboboxItem : AudioDeviceComboboxItem
    {
        public AudioOutputDeviceComboboxItem(string deviceName, Guid deviceGUID, int deviceNumber) : base(deviceName, deviceGUID, deviceNumber)
        {
            DeviceName = deviceName;
            DeviceGUID = deviceGUID;
            DeviceNumber = deviceNumber;
        }
    }

    public partial class AudioInputDeviceComboboxItem : AudioDeviceComboboxItem
    {
        public AudioInputDeviceComboboxItem(string deviceName, Guid deviceGUID, int deviceNumber) : base(deviceName, deviceGUID, deviceNumber)
        {
            DeviceName = deviceName;
            DeviceGUID = deviceGUID;
            DeviceNumber = deviceNumber;
        }
    }

    public partial class AudioInputItemViewmodel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<AudioInputDeviceComboboxItem> _audioInputDevices = new();
    }

    public partial class AudioOutputItemViewmodel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<AudioOutputDeviceComboboxItem> _audioOutputDevices = new();
    }


    public sealed partial class SettingsPage : Page
    {
        public static AudioOutputItemViewmodel comboboxes_outputlist = new();
        public static AudioInputItemViewmodel comboboxes_inputlist = new();
        private bool SettingsEnabled = false;

        public void UpdateComboboxes()
        {
            UpdateAudioOutputDevices();
            UpdateAudioInputDevices();

            // Change the comboboxes to the current audio devices

            // Update Input box (comboboxes_inputlist)
            comboboxes_inputlist.AudioInputDevices.Clear();
            foreach (var device in AudioInputDevices)
            {
                comboboxes_inputlist.AudioInputDevices.Add(new AudioInputDeviceComboboxItem(device.DeviceName, device.DeviceGUID, device.DeviceNumber));
            }

            // Update Output box (comboboxes_outputlist)
            comboboxes_outputlist.AudioOutputDevices.Clear();
            foreach (var device in AudioOutputDevices)
            {
                comboboxes_outputlist.AudioOutputDevices.Add(new AudioOutputDeviceComboboxItem(device.DeviceName, device.DeviceGUID, device.DeviceNumber));
            }

            AudioDevice CurrentLocalOutputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice);
            AudioDevice CurrentVACOutputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice);
            AudioDevice CurrentInputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.InputDevice);

            // Set the selected index of the comboboxes to the current audio devices. If not found, set to default
            InputDeviceCombobox.SelectedIndex = comboboxes_inputlist.AudioInputDevices.IndexOf(comboboxes_inputlist.AudioInputDevices.FirstOrDefault(x => x.DeviceGUID == CurrentInputDevice.DeviceGUID && x.DeviceNumber == CurrentInputDevice.DeviceNumber));
            OutputDeviceCombobox.SelectedIndex = comboboxes_outputlist.AudioOutputDevices.IndexOf(comboboxes_outputlist.AudioOutputDevices.FirstOrDefault(x => x.DeviceGUID == CurrentLocalOutputDevice.DeviceGUID && x.DeviceNumber == CurrentLocalOutputDevice.DeviceNumber));
            DriverInputCombobox.SelectedIndex = comboboxes_outputlist.AudioOutputDevices.IndexOf(comboboxes_outputlist.AudioOutputDevices.FirstOrDefault(x => x.DeviceGUID == CurrentVACOutputDevice.DeviceGUID && x.DeviceNumber == CurrentVACOutputDevice.DeviceNumber));
        }

        private void UpdateToggleSwitches()
        {
            OutputMixerToggle.IsOn = SettingsManager.GetSetting<bool>(SettingsManager.SettingTypes.OutputMicrophoneToVAC);
            HearYourselfToggle.IsOn = SettingsManager.GetSetting<bool>(SettingsManager.SettingTypes.HearYourselfEnabled);
            StackedAudioToggle.IsOn = SettingsManager.GetSetting<bool>(SettingsManager.SettingTypes.AudioStackingEnabled);
            AlwaysOnTopToggle.IsOn = SettingsManager.GetSetting<bool>(SettingsManager.SettingTypes.AlwaysOnTop);
        }

        public SettingsPage()
        {
            this.InitializeComponent();

            InputDeviceCombobox.ItemsSource = comboboxes_inputlist.AudioInputDevices;
            OutputDeviceCombobox.ItemsSource = comboboxes_outputlist.AudioOutputDevices;
            DriverInputCombobox.ItemsSource = comboboxes_outputlist.AudioOutputDevices;

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

            // Get the selected item
            var selectedItem = combobox.SelectedItem as AudioDeviceComboboxItem;
            if (selectedItem != null)
            {
                   switch (combobox.Name)
                {
                    case "InputDeviceCombobox":
                        SettingsManager.SetSetting(SettingsManager.SettingTypes.InputDevice, new AudioDevice { DeviceName = selectedItem.DeviceName, DeviceGUID = selectedItem.DeviceGUID, DeviceNumber = selectedItem.DeviceNumber});
                        break;
                    case "OutputDeviceCombobox":
                        SettingsManager.SetSetting(SettingsManager.SettingTypes.LocalOutputDevice, new AudioDevice { DeviceName = selectedItem.DeviceName, DeviceGUID = selectedItem.DeviceGUID, DeviceNumber = selectedItem.DeviceNumber });
                        break;
                    case "DriverInputCombobox":
                        SettingsManager.SetSetting(SettingsManager.SettingTypes.VACOutputDevice, new AudioDevice { DeviceName = selectedItem.DeviceName, DeviceGUID = selectedItem.DeviceGUID, DeviceNumber = selectedItem.DeviceNumber });
                        break;
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
                case "AlwaysOnTopToggle":
                    SettingsManager.SetSetting(SettingsManager.SettingTypes.AlwaysOnTop, toggleSwitch.IsOn);
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
