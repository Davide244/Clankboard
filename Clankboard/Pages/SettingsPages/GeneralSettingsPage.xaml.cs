using Clankboard.AudioSystem;
using Clankboard.Systems;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GeneralSettingsPage : Page
    {
        private SettingsSystemViewmodel settingsViewmodel = SettingsSystemViewmodel.Instance;
        private AudioDevicePickerViewModel audioDevicePickerViewModel = new AudioDevicePickerViewModel();

        public GeneralSettingsPage()
        {
            this.InitializeComponent();

            // Update audio devices
            App.appAudioDeviceManager.UpdateInputDevices();
            App.appAudioDeviceManager.UpdateOutputDevices();

            // Set data sources
            outputDeviceComboBox.ItemsSource = audioDevicePickerViewModel.OutputDevices;

            // Add output devices to the dropdown ObservableCollections
            foreach (MMDevice device in App.appAudioDeviceManager.availableOutputDevices)
            {
                mmresIconDeviceTypeInformation iconInfo = App.appAudioDeviceManager.GetDeviceTypeIconInformation(device);
                audioDevicePickerViewModel.OutputDevices.Add(new AudioDevicePickerDropdownItem(device.FriendlyName, device.ID, iconInfo.iconName, true, iconInfo.iconGlyph, iconInfo.iconFontFamily));
            }

            // console.writeline the names of the out devices in the viewmodel
            foreach (AudioDevicePickerDropdownItem item in audioDevicePickerViewModel.OutputDevices)
            {
                Debug.WriteLine("DEVICE NAME: " + item.DeviceName);
            }
        }
    }

    public partial class AudioDevicePickerDropdownItem : ObservableObject
    {
        [ObservableProperty]
        private string _deviceName;
        [ObservableProperty]
        private string _deviceID;

        [ObservableProperty]
        private string _deviceType;

        [ObservableProperty]
        private bool _isSelectable = true;

        [ObservableProperty]
        private string _iconGlyph;

        [ObservableProperty]
        private string _iconFontFamily;

        public AudioDevicePickerDropdownItem(string deviceName, string deviceID, string deviceType, bool isSelectable, string iconGlyph, string iconFontFamily = null)
        {
            DeviceName = deviceName;
            DeviceID = deviceID;
            DeviceType = deviceType;
            IsSelectable = isSelectable;
            IconGlyph = iconGlyph;
            IconFontFamily = iconFontFamily;
        }
    }

    public partial class AudioDevicePickerViewModel : ObservableObject 
    {
        [ObservableProperty]
        private ObservableCollection<AudioDevicePickerDropdownItem> _outputDevices = new ObservableCollection<AudioDevicePickerDropdownItem>();

        [ObservableProperty]
        private ObservableCollection<AudioDevicePickerDropdownItem> _localOutputDevices = new ObservableCollection<AudioDevicePickerDropdownItem>();

        [ObservableProperty]
        private ObservableCollection<AudioDevicePickerDropdownItem> _inputDevices = new ObservableCollection<AudioDevicePickerDropdownItem>();
    }
}
