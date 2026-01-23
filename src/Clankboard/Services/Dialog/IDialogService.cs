using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace Clankboard.Services.Dialog;

/// <summary>
/// Options for configuring a content dialog.
/// </summary>
public class DialogOptions
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string CloseButtonText { get; set; } = "Close";
    public string? PrimaryButtonText { get; set; }
    public string? SecondaryButtonText { get; set; }
    public ContentDialogButton DefaultButton { get; set; } = ContentDialogButton.None;
    
    /// <summary>
    /// Custom content to display in the dialog (e.g., a UserControl or Page).
    /// When set, this takes precedence over the Content string.
    /// </summary>
    public object? CustomContent { get; set; }
    
    /// <summary>
    /// Initial state for the primary button. Default is true (enabled).
    /// </summary>
    public bool IsPrimaryButtonEnabled { get; set; } = true;
    
    /// <summary>
    /// Initial state for the secondary button. Default is true (enabled).
    /// </summary>
    public bool IsSecondaryButtonEnabled { get; set; } = true;
}

/// <summary>
/// Service for displaying dialogs in a clean, injectable manner.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Gets the currently displayed dialog, if any.
    /// </summary>
    ContentDialog? CurrentDialog { get; }
    
    /// <summary>
    /// Shows a dialog with the specified options.
    /// </summary>
    Task<ContentDialogResult> ShowAsync(DialogOptions options);

    /// <summary>
    /// Shows a simple message dialog with just a close button.
    /// </summary>
    Task ShowMessageAsync(string title, string message);

    /// <summary>
    /// Shows a confirmation dialog with primary and close buttons.
    /// </summary>
    Task<bool> ShowConfirmationAsync(string title, string message, string confirmText = "Confirm", string cancelText = "Cancel");

    /// <summary>
    /// Shows a dialog with custom content.
    /// </summary>
    Task<ContentDialogResult> ShowCustomAsync(string title, object content, DialogOptions? options = null);
    
    /// <summary>
    /// Sets the enabled state of the primary button on the current dialog.
    /// </summary>
    void SetPrimaryButtonEnabled(bool enabled);
    
    /// <summary>
    /// Sets the enabled state of the secondary button on the current dialog.
    /// </summary>
    void SetSecondaryButtonEnabled(bool enabled);
}
