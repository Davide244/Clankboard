using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading;
using System.Threading.Tasks;
using Clankboard.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard
{
    public enum AppVersionType
    {
        Indev,
        Alpha,
        Beta,
        ReleaseCandidate,
        Release
    }

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static string AppDataPath;
        public static AppDataFolderManager appDataFolderManager = new();

        public const AppVersionType appVersionType = AppVersionType.Indev; // Change this to the current version type before publishing!!


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            setupAppData();
            LaunchTask();
        }

        private void setupAppData() 
        {
            string AppData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clankboard");

            // Check if the directory exists
            if (!Directory.Exists(AppData))
            {
                // Create the directory
                Directory.CreateDirectory(AppData);
            }

            AppDataPath = AppData;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        public static Window m_window;
        public NativeInterop.SplashScreen m_splashScreen;

        private async void LaunchTask()
        {
            m_splashScreen = new NativeInterop.SplashScreen();
            m_splashScreen.Initialize();

            IntPtr hBitmap = await m_splashScreen.GetBitmap(@"Assets\AppIcons\SplashScreen.scale-400.png");
            m_splashScreen.DisplaySplash(IntPtr.Zero, hBitmap, null);
        }
    }
}
