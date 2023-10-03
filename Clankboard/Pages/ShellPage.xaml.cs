using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Geolocation;
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
    public static AppInfobar g_AppInfobar = new();
    public static ShellPageEvents g_ShellPageEvents = new();

    public ShellPage()
    {
        this.InitializeComponent();
        NavigationFrame.Navigate(typeof(SoundboardPage));

        g_AppMessageBox.NewMessageBox += DisplayDialog;
        g_AppInfobar.OpenInfobar += AppInfoBar_Open;
        g_ShellPageEvents.SettingsOpenClick += SettingspageButton_Click;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        RectShadow.Receivers.Add(NavFrameParent);
        AppTopBar.Translation += new Vector3(0, 0, 32);
    }

    #region Navigation
    private void SettingspageButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(typeof(Pages.SettingsPage));
        MainCommandBar.Visibility = Visibility.Collapsed;
        AppbarBackButton.Visibility = Visibility.Visible;
    }

    private void AppbarBackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.GoBack();
        if (NavigationFrame.SourcePageType == typeof(SoundboardPage))
        {
            MainCommandBar.Visibility = Visibility.Visible;
            AppbarBackButton.Visibility = Visibility.Collapsed;
        }
        System.GC.Collect(); // Collect old page
    }

    private void AppbarHomeButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(typeof(SoundboardPage));
        MainCommandBar.Visibility = Visibility.Visible;
        AppbarBackButton.Visibility = Visibility.Collapsed;
        System.GC.Collect(); // Collect old page
    }
    #endregion

    private async Task<int> DisplayDialog(object sender, RoutedEventArgs e, string Title, string Text, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText, ContentDialogButton DefaultButton, object content)
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
        if (content != null) dialog.Content = content;

        var DialogResult = await dialog.ShowAsync();
        if (DialogResult == ContentDialogResult.Primary) return 1;
        else if (DialogResult == ContentDialogResult.Secondary) return 2;
        else if (DialogResult == ContentDialogResult.None) return 0;
        else return -1;
        //return await dialog.ShowAsync();
    }

    #region Sound file management
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

    private async void DownloadSoundFile_Click(object sender, RoutedEventArgs e)
    {
        if ((await g_AppMessageBox.ShowMessagebox("Download Sound", "", "Download", "", "Cancel", ContentDialogButton.Primary, new AppContentDialogs.DownloadYoutubeVideoDialog())) == 1)
        {
            SoundboardPage.g_SoundboardEvents.AddFileURL(AppContentDialogs.DownloadYoutubeVideoDialog.CurrentNameOverride, AppContentDialogs.DownloadYoutubeVideoDialog.CurrentURL);
        }
    }
    #endregion

    private async void OpenSoundboard_Click(object sender, RoutedEventArgs e)
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();

        var window = (Application.Current as App)?.m_window as MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

        picker.FileTypeFilter.Add(".clankboard");

        var file = await picker.PickSingleFileAsync();
        if (file != null && File.Exists(file.Path))
        {
            //TODO: Open file
        }
    }

    private void AppInfoBar_Open(object sender, RoutedEventArgs e, AppInfobar.AppInfobarType type, bool Open)
    {
        switch (type)
        {
            case AppInfobar.AppInfobarType.FileDownloadInfobar:
                if (Open) AppDownloadingFilesInfobar.Visibility = Visibility.Visible; else AppDownloadingFilesInfobar.Visibility = Visibility.Collapsed;
                AppDownloadingFilesInfobar.IsOpen = Open;
                break;
            case AppInfobar.AppInfobarType.FileMissingInfobar:
                if (Open) AppMissingFilesInfobar.Visibility = Visibility.Visible; else AppMissingFilesInfobar.Visibility = Visibility.Collapsed;
                AppMissingFilesInfobar.IsOpen = Open;
                break;
            case AppInfobar.AppInfobarType.DriverMissingInfobar:
                if (Open) AppMissingDriverInfobar.Visibility = Visibility.Visible; else AppMissingDriverInfobar.Visibility = Visibility.Collapsed;
                AppMissingDriverInfobar.IsOpen = Open;
                break;
            default:
                return;
        }
    }
}
