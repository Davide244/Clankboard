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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private AppWindow m_Appwindow;
    DesktopAcrylicController acrylicController;

    public MainWindow()
    {
        this.InitializeComponent();

        m_Appwindow = GetAppWindowForCurrentWindow();

        m_Appwindow.TitleBar.ExtendsContentIntoTitleBar = true;
        m_Appwindow.TitleBar.BackgroundColor = Colors.Transparent;
        m_Appwindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
        m_Appwindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        m_Appwindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        acrylicController = new DesktopAcrylicController();
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }
}
