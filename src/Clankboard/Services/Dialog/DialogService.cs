using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Clankboard.Services.Dialog;

/// <summary>
/// Implementation of IDialogService for WinUI 3 applications.
/// </summary>
public class DialogService : IDialogService
{
    private readonly Func<XamlRoot?> _xamlRootProvider;
    private ContentDialog? _currentDialog;

    /// <summary>
    /// Creates a new DialogService instance.
    /// </summary>
    /// <param name="xamlRootProvider">
    /// A function that provides the XamlRoot for dialogs.
    /// This is typically () => App.m_window.Content.XamlRoot
    /// </param>
    public DialogService(Func<XamlRoot?> xamlRootProvider)
    {
        _xamlRootProvider = xamlRootProvider ?? throw new ArgumentNullException(nameof(xamlRootProvider));
    }

    public ContentDialog? CurrentDialog => _currentDialog;

    public async Task<ContentDialogResult> ShowAsync(DialogOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var xamlRoot = _xamlRootProvider();
        if (xamlRoot == null)
        {
            throw new InvalidOperationException("XamlRoot is not available. Ensure the window is fully loaded before showing dialogs.");
        }

        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = options.Title,
            CloseButtonText = options.CloseButtonText,
            DefaultButton = options.DefaultButton,
            IsPrimaryButtonEnabled = options.IsPrimaryButtonEnabled,
            IsSecondaryButtonEnabled = options.IsSecondaryButtonEnabled
        };

        // Set content - custom content takes precedence
        if (options.CustomContent != null)
        {
            dialog.Content = options.CustomContent;
        }
        else if (!string.IsNullOrEmpty(options.Content))
        {
            dialog.Content = options.Content;
        }

        // Set optional buttons
        if (!string.IsNullOrEmpty(options.PrimaryButtonText))
        {
            dialog.PrimaryButtonText = options.PrimaryButtonText;
        }

        if (!string.IsNullOrEmpty(options.SecondaryButtonText))
        {
            dialog.SecondaryButtonText = options.SecondaryButtonText;
        }

        _currentDialog = dialog;
        try
        {
            return await dialog.ShowAsync();
        }
        finally
        {
            _currentDialog = null;
        }
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        await ShowAsync(new DialogOptions
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            DefaultButton = ContentDialogButton.Close
        });
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string confirmText = "Confirm", string cancelText = "Cancel")
    {
        var result = await ShowAsync(new DialogOptions
        {
            Title = title,
            Content = message,
            PrimaryButtonText = confirmText,
            CloseButtonText = cancelText,
            DefaultButton = ContentDialogButton.Primary
        });

        return result == ContentDialogResult.Primary;
    }

    public async Task<ContentDialogResult> ShowCustomAsync(string title, object content, DialogOptions? options = null)
    {
        var dialogOptions = options ?? new DialogOptions();
        dialogOptions.Title = title;
        dialogOptions.CustomContent = content;

        return await ShowAsync(dialogOptions);
    }

    public void SetPrimaryButtonEnabled(bool enabled)
    {
        if (_currentDialog != null)
        {
            _currentDialog.IsPrimaryButtonEnabled = enabled;
        }
    }

    public void SetSecondaryButtonEnabled(bool enabled)
    {
        if (_currentDialog != null)
        {
            _currentDialog.IsSecondaryButtonEnabled = enabled;
        }
    }
}
