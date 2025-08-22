using Clankboard.AudioSystem;
using Clankboard.Dialogs.Viewmodels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Dialogs
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddTTSAudioDialog : Page
    {
        private static int lastSelectedComboboxIndex = -1;

        public static string userSelectedText;
        public AddTTSAudioDialogViewModel viewModel = new();

        public AddTTSAudioDialog()
        {
            InitializeComponent();

            TTSHelper.UpdateInstalledVoices();

            // Set ItemSource of voicesComboBox
            voicesComboBox.ItemsSource = TTSVoices.Instance.InstalledVoices;

            if (lastSelectedComboboxIndex != -1) voicesComboBox.SelectedIndex = lastSelectedComboboxIndex;
        }

        private void voicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lastSelectedComboboxIndex = voicesComboBox.SelectedIndex;

            if (voicesComboBox.SelectedItem != null && textTextBox.Text != string.Empty &&
                NameTextBox.Text != string.Empty)
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = true;
            else
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = false;
        }

        private void textTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (voicesComboBox.SelectedItem != null && textTextBox.Text != string.Empty &&
                NameTextBox.Text != string.Empty)
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = true;
            else
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = false;
        }

        private void NameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (voicesComboBox.SelectedItem != null && textTextBox.Text != string.Empty &&
                NameTextBox.Text != string.Empty)
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = true;
            else
                MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = false;
        }
    }

    namespace Viewmodels
    {
        public partial class AddTTSAudioDialogViewModel : ObservableObject
        {
            [ObservableProperty] private bool _embedFile;

            [ObservableProperty] private string _name;

            [ObservableProperty] private TTSVoice _selectedVoice;

            [ObservableProperty] private int _speedMultiplierValue;

            [ObservableProperty] private string _ttsText;

            [ObservableProperty] private int _volumeValue;

            public AddTTSAudioDialogViewModel()
            {
                SpeedMultiplierValue = 0;
                VolumeValue = 100;
                EmbedFile = false;
            }
        }
    }
}