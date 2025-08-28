using System;
using System.IO;
using Clankboard.AudioSystem;
using Clankboard.NativeInterop;
using Clankboard.Utils;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard;

public enum AppVersionType
{
    Indev,
    Alpha,
    Beta,
    ReleaseCandidate,
    Release
}

/// <summary>
///     Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public const AppVersionType
        appVersionType = AppVersionType.Indev; // Change this to the current version type before publishing!!

    public static string AppDataPath;
    public static AppDataFolderManager appDataFolderManager = new();
    public static ClankAudioDeviceManager appAudioDeviceManager = new();

    public static Window m_window;
    public SplashScreen m_splashScreen;

    /// <summary>
    ///     Initializes the singleton application object.  This is the first line of authored code
    ///     executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        setupAppData();
        LaunchTask();
    }

    private void setupAppData()
    {
        var AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clankboard");

        // Check if the directory exists
        if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);

        AppDataPath = AppData;
    }

    /// <summary>
    ///     Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
    }

    private async void LaunchTask()
    {
        m_splashScreen = new SplashScreen();
        m_splashScreen.Initialize();

        var hBitmap = await m_splashScreen.GetBitmap(@"Assets\AppIcons\SplashScreen.scale-400.png");
        m_splashScreen.DisplaySplash(IntPtr.Zero, hBitmap, null);
    }
}