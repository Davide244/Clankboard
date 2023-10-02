using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clankboard;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static float DpiScalingFactor = 1;

    public App()
    {
        this.InitializeComponent();
    }

    public static FrameworkElement MainRoot { get; private set; }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
        m_window.Content = new ShellPage();

        m_window.Title = "Clankboard";
        if (System.Diagnostics.Debugger.IsAttached)
            m_window.Title = "Clankboard DEBUG";

        MainRoot = m_window.Content as FrameworkElement;

        RegisterWindowMinMax(m_window);
        if (System.Diagnostics.Debugger.IsAttached)
            MakeWindowAlwaysOnTop(m_window);
    }

    internal Window m_window;


    // Override of the WndProc method to change the window's min size
    #region WndProc
    private static WinProc newWndProc = null;
    private static IntPtr oldWndProc = IntPtr.Zero;
    private delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("User32.dll")]
    internal static extern int GetDpiForWindow(IntPtr hwnd);
    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);
    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    public static int MinWindowWidth { get; set; } = 380;//497;
    public static int MinWindowHeight { get; set; } = 635;

    private static void RegisterWindowMinMax(Window window)
    {
        var hwnd = GetWindowHandleForCurrentWindow(window);

        newWndProc = new WinProc(WndProc);
        oldWndProc = SetWindowLongPtr(hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
    }

    private static IntPtr GetWindowHandleForCurrentWindow(object target) =>
        WinRT.Interop.WindowNative.GetWindowHandle(target);

    private static IntPtr WndProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
       


        switch (Msg)
        {
            case WindowMessage.WM_GETMINMAXINFO:
                var dpi = GetDpiForWindow(hWnd);
                DpiScalingFactor = (float)dpi / 96;

                var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize.x = (int)(MinWindowWidth * DpiScalingFactor);
                minMaxInfo.ptMinTrackSize.y = (int)(MinWindowHeight * DpiScalingFactor);

                Marshal.StructureToPtr(minMaxInfo, lParam, true);
                break;

        }
        return CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
    }

    private static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
    {
        if (IntPtr.Size == 8)
            return SetWindowLongPtr64(hWnd, nIndex, newProc);
        else
            return new IntPtr(SetWindowLong32(hWnd, nIndex, newProc));
    }
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    public void MakeWindowAlwaysOnTop(Window window)
    {
        SetWindowPos(GetWindowHandleForCurrentWindow(window), (IntPtr)(-1), 0, 0, 0, 0, 0x0002 | 0x0001);
    }

    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [Flags]
    private enum WindowLongIndexFlags : int
    {
        GWL_WNDPROC = -4,
    }

    private enum WindowMessage : int
    {
        WM_GETMINMAXINFO = 0x0024,
    }
    #endregion
}
