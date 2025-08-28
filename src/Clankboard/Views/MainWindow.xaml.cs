using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Clankboard.Dialogs;
using Clankboard.Pages;
using Clankboard.Systems;
using Clankboard.Utils.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Navigation;
using WinUIEx;

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

    public static AppMessagingEvents g_appMessagingEvents = new();
    public static AppContentDialogProperties g_appContentDialogProperties = new();

    public static MainWindowInfobarViewmodel infobarViewmodel = new();

    //public static AuxSoftwareMgr g_auxSoftwareMgr = new();
    private SettingsSystemViewmodel settingsViewmodel = SettingsSystemViewmodel.Instance; // Used for the mute toggler

    public MainWindow()
    {
        InitializeComponent();
        NavigationFrame.Navigate(typeof(SoundboardPage));

        //this.PersistenceId = "ClankMainWindow";

        var appWindow = AppWindow;
        appWindow.Title = "Clankboard";
        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        g_appMessagingEvents.AppShowMessageBox += AppMessagingEvents_AppShowMessageBox;

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

    public static ContentDialog dialog { get; private set; }

    private async Task<ContentDialogResult> AppMessagingEvents_AppShowMessageBox(object sender, RoutedEventArgs e,
        string Title, string Text, string CloseButtonText, string PrimaryButtonText, string SecondaryButtonText,
        ContentDialogButton DefaultButton = ContentDialogButton.None, object content = null)
    {
        // Disable all buttons
        g_appContentDialogProperties.IsPrimaryButtonEnabled = false;
        g_appContentDialogProperties.IsSecondaryButtonEnabled = false;

        Debug.WriteLine("AppMessagingEvents_AppShowMessageBox");

        dialog = new ContentDialog();

        dialog.XamlRoot = rootGrid.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = Title;
        if (PrimaryButtonText != null) dialog.PrimaryButtonText = PrimaryButtonText;
        if (SecondaryButtonText != null) dialog.SecondaryButtonText = SecondaryButtonText;
        dialog.CloseButtonText = CloseButtonText;
        dialog.DefaultButton = DefaultButton;
        if (content != null) dialog.Content = content;

        var bindingPrimaryButtonEnabled = new Binding();
        bindingPrimaryButtonEnabled.Source = g_appContentDialogProperties;
        bindingPrimaryButtonEnabled.Path = new PropertyPath("IsPrimaryButtonEnabled");
        bindingPrimaryButtonEnabled.Mode = BindingMode.OneWay;
        dialog.SetBinding(ContentDialog.IsPrimaryButtonEnabledProperty, bindingPrimaryButtonEnabled);

        var bindingSecondaryButtonEnabled = new Binding();
        bindingSecondaryButtonEnabled.Source = g_appContentDialogProperties;
        bindingSecondaryButtonEnabled.Path = new PropertyPath("IsSecondaryButtonEnabled");
        bindingSecondaryButtonEnabled.Mode = BindingMode.OneWay;
        dialog.SetBinding(ContentDialog.IsSecondaryButtonEnabledProperty, bindingSecondaryButtonEnabled);

        return await dialog.ShowAsync();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        //NavigationFrame.Navigate(typeof(Pages.SettingsPage));

        //g_appMessagingEvents.ShowMessageBox("", "", "", null, null, ContentDialogButton.None, new Dialogs.AuxSoftwareUpdatingDialog());

        if (NavigationFrame.Content is SettingsPage)
        {
            NavigationFrame.GoBack();
            TitlebarSettingsButton.Label = "Settings";
            TitlebarSettingsButtonIcon.Glyph = settingIcon;
        }
        else
        {
            NavigationFrame.Navigate(typeof(SettingsPage));
            TitlebarSettingsButton.Label = "Back    ";
            TitlebarSettingsButtonIcon.Glyph = backIcon;
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

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        AboutDialog aboutDialog = new();
        g_appMessagingEvents.ShowMessageBox("", "", "Okay", null, null, ContentDialogButton.Close, aboutDialog);
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