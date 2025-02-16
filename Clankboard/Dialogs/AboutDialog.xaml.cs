using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutDialog : Page
    {
        public AboutDialog()
        {
            this.InitializeComponent();
            versionText.Text = String.Format("Version: {0}.{1}.{2} ",
                Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor,
                Package.Current.Id.Version.Build);

            // Set the app version type text
            switch (App.appVersionType)
            {
                case AppVersionType.Indev:
                    versionText.Text += "Indev";
                    break;
                case AppVersionType.Alpha:
                    versionText.Text += "Alpha";
                    break;
                case AppVersionType.Beta:
                    versionText.Text += "Beta";
                    break;
                case AppVersionType.ReleaseCandidate:
                    versionText.Text += "Release Candidate";
                    break;
                case AppVersionType.Release:
                    versionText.Text += "Release";
                    break;
            }
        }
    }
}
