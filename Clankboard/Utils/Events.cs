using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Clankboard.Utils.Events;

public partial class AppContentDialogProperties : ObservableObject
{
    [ObservableProperty] public bool isPrimaryButtonEnabled;

    [ObservableProperty] public bool isSecondaryButtonEnabled;
}

// App messaging events
public class AppMessagingEvents
{
    public delegate Task<ContentDialogResult> AppShowMessageBoxEventHandler(object sender, RoutedEventArgs e,
        string Title, string Text, string CloseButtonText, string PrimaryButtonText = null,
        string SecondaryButtonText = null, ContentDialogButton DefaultButton = ContentDialogButton.None,
        object content = null);

    public event AppShowMessageBoxEventHandler AppShowMessageBox;

    public Task<ContentDialogResult> ShowMessageBox(string title, string text, string closeButtonText,
        string primaryButtonText = null, string secondaryButtonText = null,
        ContentDialogButton defaultButton = ContentDialogButton.None, object content = null)
    {
        return AppShowMessageBox?.Invoke(this, new RoutedEventArgs(), title, text, closeButtonText, primaryButtonText,
            secondaryButtonText, defaultButton, content);
    }
}