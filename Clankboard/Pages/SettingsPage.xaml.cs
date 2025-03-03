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
using Clankboard.Systems;
using Clankboard.AudioSystem;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private SettingsSystemViewmodel settingsViewmodel = SettingsSystemViewmodel.Instance;

        private ClankAudioDeviceManager audioDeviceManager = new();


        public SettingsPage()
        {
            this.InitializeComponent();

            SettingsNavigationFrame.Navigate(typeof(SettingsPages.GeneralSettingsPage));
            audioDeviceManager.UpdateOutputDevices();
        }

        private void SettingsSelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            // Go to the selected settings page
            if (SettingsSelectorBar.SelectedItem.Name == "SettingsSelectorBarGeneralSettingsPage") 
            {
                SettingsNavigationFrame.Navigate(typeof(SettingsPages.GeneralSettingsPage));
            }
            else 
            {
                SettingsNavigationFrame.Navigate(typeof(SettingsPages.SoundboardSettingsPage));
            }
        }
    }
}
