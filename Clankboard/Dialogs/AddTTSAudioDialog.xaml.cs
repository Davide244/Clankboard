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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddTTSAudioDialog : Page
    {
        private static int lastSelectedComboboxIndex = -1;

        public static string userSelectedText;
        private Viewmodels.AddTTSAudioDialogViewModel viewModel = new Viewmodels.AddTTSAudioDialogViewModel();

        public AddTTSAudioDialog()
        {
            this.InitializeComponent();

            AudioSystem.TTSHelper.UpdateInstalledVoices();

            // Set ItemSource of voicesComboBox
            voicesComboBox.ItemsSource = AudioSystem.TTSVoices.Instance.InstalledVoices;

            if (lastSelectedComboboxIndex != -1)
            {
                voicesComboBox.SelectedIndex = lastSelectedComboboxIndex;
            }
        }

        private void voicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lastSelectedComboboxIndex = voicesComboBox.SelectedIndex;

            if (voicesComboBox.SelectedItem != null && textTextBox.Text != String.Empty)
            {
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = true;
            }
            else
            {
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = false;
            }
        }

        private void textTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (voicesComboBox.SelectedItem != null && textTextBox.Text != String.Empty)
            {
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = true;
            }
            else
            {
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = false;
            }
        }
    }

    namespace Viewmodels 
    {
        public partial class AddTTSAudioDialogViewModel : ObservableObject
        {
            [ObservableProperty]
            public int _speedMultiplierValue;

            [ObservableProperty]
            public int _volumeValue;

            [ObservableProperty]
            public AudioSystem.TTSVoice _selectedVoice;

            [ObservableProperty]
            public string _ttsText;

            [ObservableProperty]
            public bool _embedFile;

            public AddTTSAudioDialogViewModel()
            {
                SpeedMultiplierValue = 0;
                VolumeValue = 100;
                EmbedFile = false;
            }
        }
    }
}
