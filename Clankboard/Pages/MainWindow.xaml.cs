using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Composition.SystemBackdrops;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Diagnostics;
using Microsoft.UI;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Documents;
using Windows.UI.WindowManagement;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using AppWindowChangedEventArgs = Microsoft.UI.Windowing.AppWindowChangedEventArgs;
using Windows.Graphics;
using Microsoft.UI.Xaml.Hosting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private AppWindow m_Appwindow;
    //DesktopAcrylicController acrylicController;

    private SizeInt32 StartingWindowSize = new(Convert.ToInt32(680 * App.DpiScalingFactor), Convert.ToInt32(1000 * App.DpiScalingFactor));

    public MainWindow()
    {
        this.InitializeComponent();

        m_Appwindow = GetAppWindowForCurrentWindow();

        m_Appwindow.TitleBar.ExtendsContentIntoTitleBar = true;
        m_Appwindow.TitleBar.BackgroundColor = Colors.Transparent;
        m_Appwindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
        m_Appwindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        m_Appwindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        //if (DesktopAcrylicController.IsSupported())
        //{
        //    //acrylicController = new DesktopAcrylicController();
        //}
        //m_Appwindow.Resize(new Windows.Graphics.SizeInt32((int)(10 * App.DpiScalingFactor), (int)(10 + App.DpiScalingFactor))); // small size to make it go to min size
        // Update the m_Appwindow.Resize call
        m_Appwindow.Resize(StartingWindowSize);
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }
}
