using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Clankboard.Dialogs;
using Clankboard.Pages;
using Clankboard.Services.Dialog;
using Clankboard.Services.Navigation;
using Clankboard.Systems;
using Clankboard.Views.Pages.Soundboard;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUIEx;
using TitleBar = Microsoft.UI.Xaml.Controls.TitleBar;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard;

/// <summary>
///     An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WindowEx
{
    private const string settingIcon = "\uE713";
    private const string backIcon = "\uE72B";

    public static MainWindowInfobarViewmodel infobarViewmodel = new();

    //public static AuxSoftwareMgr g_auxSoftwareMgr = new();
    private SettingsSystemViewmodel settingsViewmodel = SettingsSystemViewmodel.Instance; // Used for the mute toggler
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    public MainWindow()
    {
        _navigationService = App.Services.GetRequiredService<INavigationService>();
        _dialogService = App.Services.GetRequiredService<IDialogService>();
        
        InitializeComponent();
        
        // Initialize the navigation service with the Frame
        _navigationService.Frame = NavigationFrame;
        
        // Navigate to the initial page
        _navigationService.NavigateTo<SoundboardPageView>();

        //this.PersistenceId = "ClankMainWindow";

        var appWindow = AppWindow;
        appWindow.Title = "Clankboard";
        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        // Set data source for the infobar list view
        InfobarList.ItemsSource = infobarViewmodel.MainWindowInfobars;


#if DEBUG
        infobarViewmodel.MainWindowInfobars.Add(new MainWindowInfobar("Debug Mode",
            "You are running a debug build of Clankboard. Expect worse performance and bugs.",
            InfoBarSeverity.Informational));
#endif

        // Check if the user has set an output device.
        if (settingsViewmodel.SelectedOutputDeviceIndex == 0)
            infobarViewmodel.MainWindowInfobars.Add(new MainWindowInfobar("No Output Device Set",
                "You have not set an output device. The app is not able to output audio data.", InfoBarSeverity.Warning,
                false));
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        //NavigationFrame.Navigate(typeof(Pages.SettingsPage));

        //g_appMessagingEvents.ShowMessageBox("", "", "", null, null, ContentDialogButton.None, new Dialogs.AuxSoftwareUpdatingDialog());

        if (NavigationFrame.Content is SettingsPage)
        {
            _navigationService.GoBack();
            //TitlebarSettingsButton.Label = "Settings";
            //TitlebarSettingsButtonIcon.Glyph = settingIcon;
        }
        else
        {
            _navigationService.NavigateTo<SettingsPage>();
            //TitlebarSettingsButton.Label = "Back    ";
            //TitlebarSettingsButtonIcon.Glyph = backIcon;
        }
    }

    private void InfoBar_CloseButtonClick(InfoBar sender, object args)
    {
        // Turn sender into a MainWindowInfobar
        var infobar = (MainWindowInfobar)sender.DataContext;

        // Remove the infobar from the list
        infobarViewmodel.MainWindowInfobars.Remove(infobar);
    }

    private void rootGrid_Loaded(object sender, RoutedEventArgs e)
    {
        //await g_appMessagingEvents.ShowMessageBox("", "", "", null, null, ContentDialogButton.None, new Dialogs.AuxSoftwareUpdatingDialog());
    }

    private void NavigationFrame_Navigated(object sender, NavigationEventArgs e)
    {
        SettingsSystemViewmodel.Instance.Save();
    }

    private void MuteButton_Click(object sender, RoutedEventArgs e)
    {
        SettingsSystemViewmodel.Instance.Save();
    }

    private void WindowEx_Closed(object sender, WindowEventArgs args)
    {
        SettingsSystemViewmodel.Instance.Save();
    }

    private async void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        var aboutDialog = new AboutDialog();
        await _dialogService.ShowCustomAsync("About Clankboard", aboutDialog, new DialogOptions
        {
            CloseButtonText = "Okay",
            DefaultButton = ContentDialogButton.Close
        });
    }

    private void TitleBar_OnBackRequested(TitleBar sender, object args)
    {
        _navigationService.GoBack();
    }
}

public partial class MainWindowInfobar : ObservableObject
{
    [ObservableProperty] private string _actionButtonText;

    [ObservableProperty] private string _actionButtonVisibility;

    [ObservableProperty] private string _bottomScrollBarVisibity;

    [ObservableProperty] private bool _isCloseable;

    [ObservableProperty] private InfoBarSeverity _severity;

    [ObservableProperty] private string _text;

    [ObservableProperty] private string _title;

    public MainWindowInfobar(string title, string text, InfoBarSeverity severity, bool isCloseable = true,
        bool scrollBarVisible = false, string actionButtonText = "")
    {
        Title = title;
        Text = text;
        IsCloseable = isCloseable;
        Severity = severity;

        BottomScrollBarVisibity = scrollBarVisible ? "Visible" : "Collapsed";

        ActionButtonText = actionButtonText;

        // Action button visibility is collapsed if text is ""
        ActionButtonVisibility = actionButtonText == "" ? "Collapsed" : "Visible";
    }
}

public partial class MainWindowInfobarViewmodel : ObservableObject
{
    [ObservableProperty] public ObservableCollection<MainWindowInfobar> _mainWindowInfobars = new();
}