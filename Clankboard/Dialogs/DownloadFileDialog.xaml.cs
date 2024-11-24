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
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Dialogs
{

    public partial class DownloadFileDialogViewmodel : ObservableValidator, INotifyPropertyChanged
    {
        // Viewmodel to validate Textbox input for the download file dialog

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [StringLength(2048, ErrorMessage = "URL is too long.")]
        [RegularExpression(@"^((https|http|ftp)://)?(?:([A-z0-9])([A-z0-9-]{1,61})?([A-z0-9])\.)?(([A-z0-9])([A-z0-9\-]{1,61})?(?:[A-z,0-9]))\.([A-z]{2,63})(?:\/[A-z0-9$-_.+!*'(),äÄöÖüÜ""<>#%{}|\^~[\]`]{1,2048})?", ErrorMessage = "URL is invalid.")]
        public string _userInputUrl;

        [RelayCommand]
        public void DownloadFile()
        {
            ValidateAllProperties();

            // Validate URL
            if (HasErrors)
            {
                return;
            }
        }

        public void TextBoxTextChangedHandler() // Gets called every time the textBox's keydown event gets called.
        {
            System.Diagnostics.Debug.WriteLine(UserInputUrl); // This is always empty for some reason
            ValidateAllProperties();
            System.Diagnostics.Debug.WriteLine("Validated"); // This appears in the output
            if (HasErrors)
            {
                System.Diagnostics.Debug.WriteLine("Errors found!"); // This does not.. Even with wrong data inside of UserInputUrl
            }
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadFileDialog : Page
    {
        public DownloadFileDialogViewmodel downloadFileDialogViewmodel = new DownloadFileDialogViewmodel();

        // Using public static as WinUI 3 only allows for one instance of a dialog to be created.. Basically a singleton (Cheating :p)
        public static string userSelectedFileUrl;
        public static string userSelectedFileName;

        public DownloadFileDialog()
        {
            this.InitializeComponent();

            //urlTextBox.RegexPattern = @"^((https|http|ftp)://)?(?:([A-z0-9])([A-z0-9-]{1,61})?([A-z0-9])\.)?(([A-z0-9])([A-z0-9\-]{1,61})?(?:[A-z,0-9]))\.([A-z]{2,63})(?:\/[A-z0-9$-_.+!*'(),äÄöÖüÜ""<>#%{}|\^~[\]`]{1,2048})?";
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //Text.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
        }
    }
}
