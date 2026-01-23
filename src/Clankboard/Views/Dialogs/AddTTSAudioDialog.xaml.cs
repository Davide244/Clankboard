using Clankboard.AudioSystem;
using Clankboard.Dialogs.Viewmodels;
using Clankboard.Services.Dialog;
using Clankboard.Services.TTS;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
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
        // SERVICES
        private readonly ITTSService ttsService;
        private readonly IDialogService _dialogService;

        private static int lastSelectedComboboxIndex = -1;

        public static string userSelectedText;
        public AddTTSAudioDialogViewModel viewModel = new();

        public AddTTSAudioDialog(ITTSService ttsService)
        {
            this.ttsService = ttsService;
            _dialogService = App.Services.GetRequiredService<IDialogService>();

            InitializeComponent();

            // Set ItemSource of voicesComboBox
            voicesComboBox.ItemsSource = ttsService.GetTTSVoiceList().Result;

            if (lastSelectedComboboxIndex != -1) voicesComboBox.SelectedIndex = lastSelectedComboboxIndex;
        }

        private void UpdatePrimaryButtonState()
        {
            bool isValid = voicesComboBox.SelectedItem != null 
                        && !string.IsNullOrEmpty(textTextBox.Text) 
                        && !string.IsNullOrEmpty(NameTextBox.Text);
            _dialogService.SetPrimaryButtonEnabled(isValid);
        }

        private void voicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lastSelectedComboboxIndex = voicesComboBox.SelectedIndex;
            UpdatePrimaryButtonState();
        }

        private void textTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            UpdatePrimaryButtonState();
        }

        private void NameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            UpdatePrimaryButtonState();
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
                SpeedMultiplierValue = 1;
                VolumeValue = 100;
                EmbedFile = false;
            }
        }
    }
}