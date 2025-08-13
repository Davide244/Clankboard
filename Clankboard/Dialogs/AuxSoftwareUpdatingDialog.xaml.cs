using Clankboard.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard.Dialogs;

/// <summary>
///     When this dialog opens, It runs all the update checks in AuxSoftwareMgr. Also runs an ffmpeg download if
///     App.alwaysDownloadFfmpegWhenUpdating is true.
/// </summary>
public sealed partial class AuxSoftwareUpdatingDialog : Page
{
    private readonly AuxSoftwareUpdatingDialogViewmodel viewmodel = new();

    public AuxSoftwareUpdatingDialog()
    {
        InitializeComponent();

        DownloadResult result;
        //result = MainWindow.g_auxSoftwareMgr.UpdateYTDLP().Result;
        //if (result == DownloadResult.Success)
        //{
        //    viewmodel.UpdateStatusText = "YTDLP updated successfully.";
        //}
        //else
        //{
        //    viewmodel.UpdateStatusText = "YTDLP update failed.";
        //}

        // Close this dialog.
        //MainWindow.dialog.Hide();
    }
}

public partial class AuxSoftwareUpdatingDialogViewmodel : ObservableObject
{
    [ObservableProperty] private string _updateStatusText;

    public AuxSoftwareUpdatingDialogViewmodel()
    {
        UpdateStatusText = "Initializing...";
    }
}