using System;
using Clankboard;
using WinUIEx;

#if DISABLE_XAML_GENERATED_MAIN
public class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var splash = SimpleSplashScreen.ShowDefaultSplashScreen();

        WinRT.ComWrappersSupport.InitializeComWrappers();

        Microsoft.UI.Xaml.Application.Start((p) => {
            var context = new Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
            System.Threading.SynchronizationContext.SetSynchronizationContext(context);
            new App(splash); // Pass the splash screen to your app so it can close it on activation
        });

        return 0;
    }

    // TODO: App redirection. (Making this single instance.)
}

#endif