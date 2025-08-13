using System.Collections.ObjectModel;
using System.Diagnostics;
using Clankboard.Systems;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Pages.SettingsPages;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class GeneralSettingsPage : Page
{
    private readonly AudioDevicePickerViewModel audioDevicePickerViewModel = new();
    private SettingsSystemViewmodel settingsViewmodel = SettingsSystemViewmodel.Instance;

    public GeneralSettingsPage()
    {
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        // Update audio devices
        App.appAudioDeviceManager.UpdateInputDevices();
        App.appAudioDeviceManager.UpdateOutputDevices();

        // Clear viewmodel
        audioDevicePickerViewModel.OutputDevices.Clear();
        audioDevicePickerViewModel.LocalOutputDevices.Clear();
        audioDevicePickerViewModel.InputDevices.Clear();

        // Set data sources
        outputDeviceComboBox.ItemsSource = audioDevicePickerViewModel.OutputDevices;
        localOutputDeviceComboBox.ItemsSource = audioDevicePickerViewModel.LocalOutputDevices;
        inputDeviceComboBox.ItemsSource = audioDevicePickerViewModel.InputDevices;

        // Add output devices to the dropdown ObservableCollections
        foreach (var device in App.appAudioDeviceManager.availableOutputDevices)
        {
            var iconInfo = App.appAudioDeviceManager.GetDeviceTypeIconInformation(device);
            audioDevicePickerViewModel.OutputDevices.Add(new AudioDevicePickerDropdownItem(device.FriendlyName,
                device.ID, iconInfo.iconName, true, iconInfo.iconGlyph, iconInfo.iconFontFamily));
            audioDevicePickerViewModel.LocalOutputDevices.Add(new AudioDevicePickerDropdownItem(device.FriendlyName,
                device.ID, iconInfo.iconName, true, iconInfo.iconGlyph, iconInfo.iconFontFamily));
        }

        // input devices
        foreach (var device in App.appAudioDeviceManager.availableInputDevices)
        {
            var iconInfo = App.appAudioDeviceManager.GetDeviceTypeIconInformation(device);
            audioDevicePickerViewModel.InputDevices.Add(new AudioDevicePickerDropdownItem(device.FriendlyName,
                device.ID, iconInfo.iconName, true, iconInfo.iconGlyph, iconInfo.iconFontFamily));
        }

        // console.writeline the names of the out devices in the viewmodel
        foreach (var item in audioDevicePickerViewModel.OutputDevices)
            Debug.WriteLine("DEVICE NAME: " + item.DeviceName);
    }
}

public partial class AudioDevicePickerDropdownItem : ObservableObject
{
    [ObservableProperty] private string _deviceID;

    [ObservableProperty] private string _deviceName;

    [ObservableProperty] private string _deviceType;

    [ObservableProperty] private string _iconFontFamily;

    [ObservableProperty] private string _iconGlyph;

    [ObservableProperty] private bool _isSelectable = true;

    public AudioDevicePickerDropdownItem(string deviceName, string deviceID, string deviceType, bool isSelectable,
        string iconGlyph, string iconFontFamily = null)
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
    [ObservableProperty] private ObservableCollection<AudioDevicePickerDropdownItem> _inputDevices = new();

    [ObservableProperty] private ObservableCollection<AudioDevicePickerDropdownItem> _localOutputDevices = new();

    [ObservableProperty] private ObservableCollection<AudioDevicePickerDropdownItem> _outputDevices = new();
}