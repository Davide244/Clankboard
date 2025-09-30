

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard
{
    using System;
    using System.IO;
    using AudioSystem;
    using Microsoft.UI.Xaml;
    using Utils;
    using WinUIEx;

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
        public const AppVersionType appVersionType = AppVersionType.Indev; // Change this to the current version type before publishing!!

        public static string AppDataPath;
        public static AppDataFolderManager appDataFolderManager = new();
        public static ClankAudioDeviceManager appAudioDeviceManager = new();
        public static Window m_window;
        private SimpleSplashScreen m_splashScreen;

        public App(SimpleSplashScreen splashScreen)
        {
            m_splashScreen = splashScreen;

            InitializeComponent();
            setupAppData();
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

            m_splashScreen?.Hide();
            m_splashScreen = null;
        }
    }
}