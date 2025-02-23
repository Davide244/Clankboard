using Clankboard.Systems;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public GeneralSettingsPage()
        {
            this.InitializeComponent();
        }
    }

    public partial class AudioDevicePickerDropdownItem : ObservableObject
    {
        [ObservableProperty]
        private string _deviceName;
        [ObservableProperty]
        private string _deviceID;

        [ObservableProperty]
        private bool _isSelectable = true;

        public AudioDevicePickerDropdownItem(string deviceName, string deviceID, bool isSelectable)
        {
            DeviceName = deviceName;
            DeviceID = deviceID;
            IsSelectable = isSelectable;
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
