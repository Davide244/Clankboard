using Clankboard.AudioSystem;
using Clankboard.Pages.SettingsPages;
using Clankboard.Systems;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Pages;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page
{
    private readonly ClankAudioDeviceManager audioDeviceManager = new();
    private SettingsSystemViewmodel settingsViewmodel = SettingsSystemViewmodel.Instance;


    public SettingsPage()
    {
        InitializeComponent();

        SettingsNavigationFrame.Navigate(typeof(GeneralSettingsPage));
        audioDeviceManager.UpdateOutputDevices();
    }

    private void SettingsSelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        // Go to the selected settings page
        if (SettingsSelectorBar.SelectedItem.Name == "SettingsSelectorBarGeneralSettingsPage")
            SettingsNavigationFrame.Navigate(typeof(GeneralSettingsPage));
        else
            SettingsNavigationFrame.Navigate(typeof(SoundboardSettingsPage));
    }
}