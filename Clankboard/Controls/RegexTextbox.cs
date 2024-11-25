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

        public RegexTextbox()
        {
            this.DefaultStyleKey = typeof(RegexTextbox);

            this.KeyDown += RegexTextbox_KeyDown;
        }

        private void RegexTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Validate the input on every keydown event by checking the regex pattern
            //if (RegexPattern != null)
            //{
            //    if (!System.Text.RegularExpressions.Regex.IsMatch(this.Text, RegexPattern))
            //    {
            //        hasErrors = true;
            //        FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedError"] as Brush;
            //        UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedError"] as Brush;
            //    }
            //    else
            //    {
            //        hasErrors = false;
            //        FocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderFocusedNoError"] as Brush;
            //        UnFocusedBorderBrush = Application.Current.Resources["RegexTextBoxBorderUnFocusedNoError"] as Brush;
            //    }
            //}

            Debug.WriteLine("Key down event");

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
