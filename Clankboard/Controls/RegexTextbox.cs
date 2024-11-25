using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Controls
{
    public sealed class RegexTextbox : TextBox
    {
        public bool hasErrors = false;

        public string RegexPattern
        {
            get { return (string)GetValue(RegexPatternProperty); }
            set { SetValue(RegexPatternProperty, value); }
        }
        // Dependency property containing the regex pattern to validate the input
        public static DependencyProperty RegexPatternProperty = DependencyProperty.Register(
            "RegexPattern",
            typeof(string),
            typeof(RegexTextbox),
            new PropertyMetadata(null)
        );

        // Foreground color of the description text
        public Brush DescriptionForeground
        {
            get { return (Brush)GetValue(DescriptionForegroundProperty); }
            set { SetValue(DescriptionForegroundProperty, value); }
        }

        public static readonly DependencyProperty DescriptionForegroundProperty = DependencyProperty.Register(
            "DescriptionForeground",
            typeof(Brush),
            typeof(RegexTextbox),
            new PropertyMetadata(null)
        );

        // EventHandler for the validityStatusChanged event
        public EventHandler validityChanged 
        {
            get { return (EventHandler)GetValue(validityChangedProperty); }
            set { SetValue(validityChangedProperty, value); }
        }

        public static readonly DependencyProperty validityChangedProperty = DependencyProperty.Register(
            "validityChanged",
            typeof(EventHandler),
            typeof(RegexTextbox),
            new PropertyMetadata(null)
        );


        // Brush for the border of the textbox when focused
        public Brush FocusedBorderBrush
        {
            get { return (Brush)GetValue(FocusedBorderBrushProperty); }
            set { SetValue(FocusedBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty FocusedBorderBrushProperty = DependencyProperty.Register(
            "FocusedBorderBrush",
            typeof(Brush),
            typeof(RegexTextbox),
            new PropertyMetadata(Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush)
        );

        // Brush for the border of the textbox when not focused
        public Brush UnFocusedBorderBrush
        {
            get { return (Brush)GetValue(UnFocusedBorderBrushProperty); }
            set { SetValue(UnFocusedBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty UnFocusedBorderBrushProperty = DependencyProperty.Register(
            "UnFocusedBorderBrush",
            typeof(Brush),
            typeof(RegexTextbox),
            new PropertyMetadata(Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush)
        );


        public RegexTextbox()
        {
            this.DefaultStyleKey = typeof(RegexTextbox);

            this.TextChanged += RegexTextbox_TextChanged;
            this.validityChanged += RegexTextbox_validityChanged;

            // Set the default border brush to RegexTextBoxBorderFocusedError
            //this.FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush;
            //this.UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush;

            this.BorderBrush = this.UnFocusedBorderBrush;
        }

        bool previousHasErrors = false; // Little hack because im too lazy to implement a proper event handler
        private void RegexTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Validate the input on every keydown event by checking the regex pattern
            if (RegexPattern != null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(this.Text, RegexPattern))
                {
                    previousHasErrors = hasErrors;
                    hasErrors = true;
                    this.DescriptionForeground = /*Change to SystemFillColorCritical*/ Application.Current.Resources["SystemFillColorCriticalBrush"] as Brush;
                    this.Description = "Please enter a valid URL.";

                    this.FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush;
                    this.UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush;
                    this.BorderBrush = this.UnFocusedBorderBrush;

                    if (previousHasErrors != hasErrors)
                        validityChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    previousHasErrors = hasErrors;
                    hasErrors = false;
                    this.DescriptionForeground = /*Change to SystemFillColorCritical*/ Application.Current.Resources["SystemFillColorSuccessBrush"] as Brush;
                    this.Description = "URL is valid.";

                    this.FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedNoError"] as Brush;
                    this.UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedNoError"] as Brush;
                    this.BorderBrush = this.UnFocusedBorderBrush;

                    if (previousHasErrors != hasErrors)
                        validityChanged?.Invoke(this, EventArgs.Empty);
                }
            }

        }

        private void RegexTextbox_validityChanged(object sender, EventArgs e)
        {
            if (hasErrors)
            {
                this.FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush;
                this.UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush;
                this.BorderBrush = this.UnFocusedBorderBrush;
            }
            else
            {
                this.FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedNoError"] as Brush;
                this.UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedNoError"] as Brush;
                this.BorderBrush = this.UnFocusedBorderBrush;
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
}
