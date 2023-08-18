using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellPage : Page
{

    public static AppMessagebox g_AppMessageBox = new();

    public ShellPage()
    {
        this.InitializeComponent();
        NavigationFrame.Navigate(typeof(SoundboardPage));

        g_AppMessageBox.NewMessageBox += DisplayDialog;
    }

    public async Task<int> DisplayDialog(object sender, RoutedEventArgs e, string Title, string Text, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText, ContentDialogButton DefaultButton)
    {

        ContentDialog dialog = new ContentDialog();
        dialog.XamlRoot = this.XamlRoot;

        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = Title;
        dialog.Content = Text;
        dialog.PrimaryButtonText = PrimaryButtonText;
        dialog.SecondaryButtonText = SecondaryButtonText;
        dialog.CloseButtonText = CloseButtonText;
        dialog.DefaultButton = DefaultButton;

        var DialogResult = await dialog.ShowAsync();
        if (DialogResult == ContentDialogResult.Primary) return 1;
        else if (DialogResult == ContentDialogResult.Secondary) return 2;
        else if (DialogResult == ContentDialogResult.None) return 0;
        else return -1;
        //return await dialog.ShowAsync();
    }

    private async void RemoveAllSounds_Click(object sender, RoutedEventArgs e)
    {
        if ((await g_AppMessageBox.ShowMessagebox("Remove Sounds", "Are you sure that you want to remove all sounds from your soundboard?\nThis action may be irreversible!", "Yes", "", "No", ContentDialogButton.Close)) == 1)
        {
            SoundboardPage.g_SoundboardEvents.DeleteAllItems();
        }
    }

    private async void AddSoundFile_Click(object sender, RoutedEventArgs e)
    {
        // Show the file picker so the user can select a file
        var picker = new Windows.Storage.Pickers.FileOpenPicker();

        var window = (Application.Current as App)?.m_window as MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

        picker.FileTypeFilter.Add(".mp3");
        picker.FileTypeFilter.Add(".wav");
        picker.FileTypeFilter.Add(".ogg");
        picker.FileTypeFilter.Add(".mp4");

        var file = await picker.PickSingleFileAsync();
        if (file != null && File.Exists(file.Path))
        {
            SoundboardPage.g_SoundboardEvents.AddFile(Path.GetFileNameWithoutExtension(file.Name), file.Path);
        }
        //else if (file != null)
            //await DisplayDialog("File not found", "The specified file could not be found!\nPlease check if the file exists and try again.", "", "", "Okay", ContentDialogButton.Close);
    }
}
