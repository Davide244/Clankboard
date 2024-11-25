using Clankboard.Utils.Events;
using Microsoft.UI;
using Microsoft.UI.Windowing;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {

        public static AppMessagingEvents g_appMessagingEvents = new();
        public static AppContentDialogProperties g_appContentDialogProperties = new();

        private ContentDialog dialog;

        public MainWindow()
        {
            this.InitializeComponent();
            NavigationFrame.Navigate(typeof(Pages.SoundboardPage));

            //this.PersistenceId = "ClankMainWindow";

            AppWindow appWindow = this.AppWindow;
            appWindow.Title = "Clankboard";
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

            g_appMessagingEvents.AppShowMessageBox += AppMessagingEvents_AppShowMessageBox;
        }

        private async Task<ContentDialogResult> AppMessagingEvents_AppShowMessageBox(object sender, RoutedEventArgs e, string Title, string Text, string CloseButtonText, string PrimaryButtonText, string SecondaryButtonText, ContentDialogButton DefaultButton = ContentDialogButton.None, object content = null)
        {
            // Disable all buttons
            g_appContentDialogProperties.IsPrimaryButtonEnabled = false;
            g_appContentDialogProperties.IsSecondaryButtonEnabled = false;

            System.Diagnostics.Debug.WriteLine("AppMessagingEvents_AppShowMessageBox");

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

        private void rootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // For testing: Display the download file dialog
            g_appMessagingEvents.ShowMessageBox("Download File", "", "Cancel", "Download File", "", ContentDialogButton.Primary, new Dialogs.DownloadFileDialog());
        }
    }
}
