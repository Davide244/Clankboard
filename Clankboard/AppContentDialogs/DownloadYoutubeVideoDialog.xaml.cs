using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using YoutubeExplode.Videos;

namespace Clankboard.AppContentDialogs;

public sealed partial class DownloadYoutubeVideoDialog : Page
{
    public static string CurrentURL = ""; // This static variable works because there can only be one instance of this dialog at a time due to winui 3 limitations.
    public static string CurrentNameOverride = "";

    public DownloadYoutubeVideoDialog()
    {
        this.InitializeComponent();
        CurrentNameOverride = "";
        CurrentURL = "";
    }

    // TextBox TextChanged event
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!URLTextBox.Text.Contains("youtube.com/watch?v=") && !URLTextBox.Text.Contains("youtu.be/"))
            InvalidURLText.Visibility = Visibility.Visible;
        else
        {
            InvalidURLText.Visibility = Visibility.Collapsed;
            CurrentURL = URLTextBox.Text;
        }
    }

    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (NameTextBox.Text != "") CurrentNameOverride = NameTextBox.Text;
    }
    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        NameTextBox.Visibility = Visibility.Visible;
        CurrentNameOverride = NameTextBox.Text;
    }
    private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
    {
        NameTextBox.Visibility = Visibility.Collapsed;
        CurrentNameOverride = "";
    }
}
