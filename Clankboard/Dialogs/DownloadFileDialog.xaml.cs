using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Dialogs;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DownloadFileDialog : Page
{
    // Using public static as WinUI 3 only allows for one instance of a dialog to be created.. Basically a singleton (Cheating :p)
    public static string userSelectedFileUrl;
    public static string userSelectedFileName;
    public static bool overrideFileName;

    public DownloadFileDialog()
    {
        InitializeComponent();

        urlTextBox.RegexPattern =
            @"^((https|http|ftp)://)?(?:([A-z0-9])([A-z0-9-]{1,61})?([A-z0-9])?\.)?(([A-z0-9])([A-z0-9\-]{1,61})?(?:[A-z,0-9]))\.([A-z]{2,63})(?:\/[A-z0-9$-_.+!*'(),äÄöÖüÜ""<>#%{}|\^~[\]`]{1,2048})?";
        urlTextBox.validityChanged += urlTextBox_validityChanged;
    }

    private void urlTextBox_validityChanged(object sender, EventArgs e)
    {
        if (urlTextBox.hasErrors)
            MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = false;
        else
            MainWindow.g_appContentDialogProperties.IsPrimaryButtonEnabled = true;
    }

    private void urlTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        userSelectedFileUrl = urlTextBox.Text;
    }

    private void overrideFileNameCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        fileNameTextBox.IsEnabled = overrideFileNameCheckBox.IsChecked.Value;
        overrideFileName = overrideFileNameCheckBox.IsChecked.Value;
    }

    private void fileNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        userSelectedFileName = fileNameTextBox.Text;
    }
}