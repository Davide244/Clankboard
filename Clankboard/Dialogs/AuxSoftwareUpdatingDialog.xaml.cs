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
using CommunityToolkit.Mvvm.ComponentModel;
using Clankboard.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Dialogs
{
    /// <summary>
    /// When this dialog opens, It runs all the update checks in AuxSoftwareMgr. Also runs an ffmpeg download if App.alwaysDownloadFfmpegWhenUpdating is true.
    /// </summary>
    public sealed partial class AuxSoftwareUpdatingDialog : Page
    {
        private AuxSoftwareUpdatingDialogViewmodel viewmodel = new();

        public AuxSoftwareUpdatingDialog()
        {
            this.InitializeComponent();

            DownloadResult result;
            result = MainWindow.g_auxSoftwareMgr.UpdateYTDLP().Result;
            if (result == DownloadResult.Success)
            {
                viewmodel.UpdateStatusText = "YTDLP updated successfully.";
            }
            else
            {
                viewmodel.UpdateStatusText = "YTDLP update failed.";
            }

            // Close this dialog.
            //MainWindow.dialog.Hide();
        }
    }

    public partial class AuxSoftwareUpdatingDialogViewmodel : ObservableObject 
    {
        [ObservableProperty]
        private string _updateStatusText;

        public AuxSoftwareUpdatingDialogViewmodel() 
        {
            UpdateStatusText = "Initializing...";
        }
    }
}
