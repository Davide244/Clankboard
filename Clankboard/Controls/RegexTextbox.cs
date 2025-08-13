using System;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Controls;

public sealed class RegexTextbox : TextBox
{
    // Dependency property containing the regex pattern to validate the input
    public static DependencyProperty RegexPatternProperty = DependencyProperty.Register(
        "RegexPattern",
        typeof(string),
        typeof(RegexTextbox),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty DescriptionForegroundProperty = DependencyProperty.Register(
        "DescriptionForeground",
        typeof(Brush),
        typeof(RegexTextbox),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty validityChangedProperty = DependencyProperty.Register(
        "validityChanged",
        typeof(EventHandler),
        typeof(RegexTextbox),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty FocusedBorderBrushProperty = DependencyProperty.Register(
        "FocusedBorderBrush",
        typeof(Brush),
        typeof(RegexTextbox),
        new PropertyMetadata(Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush)
    );

    public static readonly DependencyProperty UnFocusedBorderBrushProperty = DependencyProperty.Register(
        "UnFocusedBorderBrush",
        typeof(Brush),
        typeof(RegexTextbox),
        new PropertyMetadata(Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush)
    );

    public bool hasErrors;

    private bool previousHasErrors; // Little hack because im too lazy to implement a proper event handler


    public RegexTextbox()
    {
        DefaultStyleKey = typeof(RegexTextbox);

        TextChanged += RegexTextbox_TextChanged;
        validityChanged += RegexTextbox_validityChanged;

        // Set the default border brush to RegexTextBoxBorderFocusedError
        //this.FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush;
        //this.UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush;

        BorderBrush = UnFocusedBorderBrush;
    }

    public string RegexPattern
    {
        get => (string)GetValue(RegexPatternProperty);
        set => SetValue(RegexPatternProperty, value);
    }

    // Foreground color of the description text
    public Brush DescriptionForeground
    {
        get => (Brush)GetValue(DescriptionForegroundProperty);
        set => SetValue(DescriptionForegroundProperty, value);
    }

    // EventHandler for the validityStatusChanged event
    public EventHandler validityChanged
    {
        get => (EventHandler)GetValue(validityChangedProperty);
        set => SetValue(validityChangedProperty, value);
    }


    // Brush for the border of the textbox when focused
    public Brush FocusedBorderBrush
    {
        get => (Brush)GetValue(FocusedBorderBrushProperty);
        set => SetValue(FocusedBorderBrushProperty, value);
    }

    // Brush for the border of the textbox when not focused
    public Brush UnFocusedBorderBrush
    {
        get => (Brush)GetValue(UnFocusedBorderBrushProperty);
        set => SetValue(UnFocusedBorderBrushProperty, value);
    }

    private void RegexTextbox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Validate the input on every keydown event by checking the regex pattern
        if (RegexPattern != null)
        {
            if (!Regex.IsMatch(Text, RegexPattern))
            {
                previousHasErrors = hasErrors;
                hasErrors = true;
                DescriptionForeground = /*Change to SystemFillColorCritical*/
                    Application.Current.Resources["SystemFillColorCriticalBrush"] as Brush;
                Description = "Please enter a valid URL.";

                FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush;
                UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush;
                BorderBrush = UnFocusedBorderBrush;

                if (previousHasErrors != hasErrors)
                    validityChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                previousHasErrors = hasErrors;
                hasErrors = false;
                DescriptionForeground = /*Change to SystemFillColorCritical*/
                    Application.Current.Resources["SystemFillColorSuccessBrush"] as Brush;
                Description = "URL is valid.";

                FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedNoError"] as Brush;
                UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedNoError"] as Brush;
                BorderBrush = UnFocusedBorderBrush;

                if (previousHasErrors != hasErrors)
                    validityChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void RegexTextbox_validityChanged(object sender, EventArgs e)
    {
        if (hasErrors)
        {
            FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush;
            UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush;
            BorderBrush = UnFocusedBorderBrush;
        }
        else
        {
            FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedNoError"] as Brush;
            UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedNoError"] as Brush;
            BorderBrush = UnFocusedBorderBrush;
        }

        // What do I do here?
    }

    //private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    // If the regex pattern changes, validate the input again
    //    if (d is RegexTextbox regexTextbox)
    //    {
    //        if (!System.Text.RegularExpressions.Regex.IsMatch(regexTextbox.Text, regexTextbox.RegexPattern))
    //        {
    //            regexTextbox.hasErrors = true;
    //            regexTextbox.FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush;
    //            regexTextbox.UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush;
    //        }
    //        else
    //        {
    //            regexTextbox.hasErrors = false;
    //            regexTextbox.FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedNoError"] as Brush;
    //            regexTextbox.UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedNoError"] as Brush;
    //        }
    //    }
    //}
}