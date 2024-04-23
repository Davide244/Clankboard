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

namespace Clankboard.AppContentDialogs
{
    // Little viewmodel that says if the primary button is enabled
    
    public partial class AppContentDialogProperties : ObservableObject
    {
        [ObservableProperty]
        public bool isPrimaryButtonEnabled;

        [ObservableProperty]
        public bool isSecondaryButtonEnabled;
    }

    /// <summary>
    /// This dialog is used to save a soundboard to a file. CONTEXT: ContentDialog
    /// </summary>
    public sealed partial class SaveSoundboardFileDialog : Page
    {
        public static string CurrentFilePath = "";
        public static bool CurrentEmbedLocalFilesEnabled = false;
        public static bool CurrentEmbedDownloadedFilesEnabled = false;


        public SaveSoundboardFileDialog()
        {
            this.InitializeComponent();

            // Disable primary button of dialog until a file name is entered (IsPrimaryButtonEnabled)
            ShellPage.g_AppContentDialogProperties.IsPrimaryButtonEnabled = false;
        }

        /// <summary>
        /// This function checks if the file name is valid and enables the primary button if it is. It does this by checking
        /// if the folder the selected file is in exists and if the file name is not empty. It also checks for invalid characters.
        /// </summary>
        /// <param name="Path">The file path.</param>
        private void ValidateFilePath(string path)
        {
            // Check if the file name is empty
            if (string.IsNullOrEmpty(path))
            {
                ShellPage.g_AppContentDialogProperties.IsPrimaryButtonEnabled = false;
                return;
            }

            // check folder path using GetInvalidPathChars
            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ShellPage.g_AppContentDialogProperties.IsPrimaryButtonEnabled = false;
                return;
            }

            // Check if the file name contains invalid characters
            string fileName = Path.GetFileName(path);
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ShellPage.g_AppContentDialogProperties.IsPrimaryButtonEnabled = false;
                return;
            }

            // Check if the folder exists
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                ShellPage.g_AppContentDialogProperties.IsPrimaryButtonEnabled = false;
                return;
            }

            // If all checks pass, enable the primary button
            ShellPage.g_AppContentDialogProperties.IsPrimaryButtonEnabled = true;
        }

        // FilePickerTextBox TextChanged event
        private void FilePickerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FilePathTextBox.Text != "" && FilePathTextBox.isPathValid())
            {
                ShellPage.g_AppContentDialogProperties.IsPrimaryButtonEnabled = true;
                CurrentFilePath = FilePathTextBox.Text;
            }
            else
                ShellPage.g_AppContentDialogProperties.IsPrimaryButtonEnabled = false;
        }

        // Checkbox Checked event (Change according to sender)
        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if ((CheckBox)sender == EmbedLocalFilesCheckBox)
                CurrentEmbedLocalFilesEnabled = true;
            else if ((CheckBox)sender == EmbedDownloadedFilesCheckbox)
                CurrentEmbedDownloadedFilesEnabled = true;
        }

        // Checkbox Unchecked event (Change according to sender)
        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((CheckBox)sender == EmbedLocalFilesCheckBox)
                CurrentEmbedLocalFilesEnabled = false;
            else if ((CheckBox)sender == EmbedDownloadedFilesCheckbox)
                CurrentEmbedDownloadedFilesEnabled = false;
        }
    }
}
