using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard;

public class SoundBoardItem
{
    public string SoundName { get; set; }

    public string SoundLocation { get; set; }
    public string SoundLocationIcon { get; set; }
    public string SoundIconColor { get; set; }
    public string SoundIconVisible { get; set; }
    public string SoundIconTooltip { get; set; }

    public string SoundKeybindForecolor { get; set; }
    public string SoundKeybind { get; set; }
    public bool ProgressRingEnabled { get; set; }
    public bool BtnEnabled { get; set; }

    public SoundBoardItem(string soundName, string soundLocation, string soundLocationIcon, bool soundIconVisible, string soundIconTooltip, string soundKeybind, bool progressRingEnabled, bool btnEnabled, string soundIconColor = null, string soundKeybindForecolor = null)
    {
    
        SoundName = soundName;
        SoundLocation = soundLocation;
        SoundLocationIcon = soundLocationIcon;
        SoundIconColor = soundIconColor;
        SoundIconTooltip = soundIconTooltip;
        SoundKeybindForecolor = soundKeybindForecolor;
        SoundKeybind = soundKeybind;
        ProgressRingEnabled = progressRingEnabled;
        BtnEnabled = btnEnabled;
        if (soundIconVisible == true) { SoundIconVisible = "Visible"; } else { SoundIconVisible = "Collapsed"; }
    }
}
public class SoundBoardItemViewmodel
{
    public List<SoundBoardItem> SoundBoardItems = new();
}

public sealed partial class SoundboardPage : Page
{
    public SoundBoardItemViewmodel soundBoardItemViewmodel = new();
    const string LocalFileIcon = "\uE8A5";
    const string WarningFileIcon = "\uE783";
    const string DownloadedFileIcon = "\uE753";
    const string DownloadingFileIcon = "\uEBD3";

    public SoundboardPage()
    {
        this.InitializeComponent();
        MainSoundboardListview.ItemsSource = soundBoardItemViewmodel.SoundBoardItems;
        soundBoardItemViewmodel.SoundBoardItems.Add(new SoundBoardItem("Sound 1", "C:\\Users\\User\\Desktop\\Clankboard\\Clankboard\\Assets\\Sound1.mp3", "\uE8A5", true, "Local File", "Ctrl + 1", false, true));
        soundBoardItemViewmodel.SoundBoardItems.Add(new SoundBoardItem("Sound 2", "C:\\Users\\User\\Desktop\\Clankboard\\Clankboard\\Assets\\Sound2.mp3", "\uE753", true, "Downloaded File", "None", false, true));
        soundBoardItemViewmodel.SoundBoardItems.Add(new SoundBoardItem("Sound 3", "Downloading: 47%", "\uE753", false, "Downloaded File", "None", true, false));
    }

    public void AddSoundFile(string Name, string FilePath)
    {
        if (!Regex.IsMatch(FilePath, "^.*\\.(mp3|.ogg|.wav|.mp4)$", RegexOptions.IgnoreCase)) { return; }

        if (File.Exists(FilePath))
        {
            if (Regex.IsMatch(Name, "[A-Za-z0-9_!*������#+�?-]+"))
            {
                soundBoardItemViewmodel.SoundBoardItems.Add(new SoundBoardItem(Name, FilePath, LocalFileIcon, true, "Local File", "None", false, true));
            }
        }
        else { AppSecondaryInfobar.IsOpen = true; }
    }

    public void DownloadSoundFile(string Name, string Url)
    {
        throw new NotImplementedException();
    }
}
