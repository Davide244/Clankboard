using Clankboard.Controls;
using CommunityToolkit.WinUI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using NAudio.Mixer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Clankboard.Classes
{
    /// <summary>
    /// This class manages the keybinds for the soundboard and interacts with win32 to register global keybinds
    /// </summary>
    public static class KeybindsManager
    {
        public enum KeybindTypes
        {
            StopAllSoundsKeybind,
            PlaySoundKeybind
        };

        /// <summary>
        /// This enum is used to define the key modifiers for the keybinds and link them to the win32 key modifiers.
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey">See for modifier list.</see>
        /// </summary>
        public enum KeyModifiers
        {
            Control = (int)Windows.Win32.UI.Input.KeyboardAndMouse.HOT_KEY_MODIFIERS.MOD_CONTROL,
            Shift = (int)Windows.Win32.UI.Input.KeyboardAndMouse.HOT_KEY_MODIFIERS.MOD_SHIFT,
            Alt = (int)Windows.Win32.UI.Input.KeyboardAndMouse.HOT_KEY_MODIFIERS.MOD_ALT,
            Win = (int)Windows.Win32.UI.Input.KeyboardAndMouse.HOT_KEY_MODIFIERS.MOD_WIN,
        }


        public struct Keybind
        {
            public KeybindTypes KeybindType;
            public List<KeyModifiers> KeyModifiers;        // Shift, Ctrl, Alt, Win, Menu, etc. (In a list of course)
            public VirtualKey Key;                         // The actual key
            public int GlobalKeybindID;                    // The global keybind ID
            public Action Handler;                         // The function that runs when the keybind is pressed, takes a parameter of SoundBoardItem that can be null
        }

        // Illegal key List
        public static readonly List<VirtualKey> IllegalKeys = new List<VirtualKey> { 
            VirtualKey.Escape, 
            VirtualKey.LeftButton, 
            VirtualKey.RightButton, 
            VirtualKey.LeftWindows, 
            VirtualKey.RightWindows, 
            VirtualKey.LeftControl, 
            VirtualKey.RightControl, 
            VirtualKey.Control,
            VirtualKey.LeftMenu, 
            VirtualKey.RightMenu,
            VirtualKey.Menu,
            VirtualKey.LeftShift, 
            VirtualKey.RightShift,
            VirtualKey.Shift};

        /// <summary>
        /// The global keybind list
        /// </summary>
        public static List<Keybind> Keybinds { get; private set; } = new List<Keybind> { };

        /// <summary>
        /// Are keybinds enabled?
        /// Use this to block input when a dialog is open to prevent the app from crashing.
        /// </summary>
        public static bool KeybindsEnabled { get; set; } = true;

        /// <summary>
        /// Add a keybind to the keybind list
        /// </summary>
        /// <param name="KeybindType">The keybind's type</param>
        /// <param name="KeyModifiers">List of modifier keys the keybind is using</param>
        /// <param name="Key">The actual key of the keybind</param>
        /// <param name="HandlerFunction">The Action that runs after the key was pressed</param>
        /// <param name="GlobalKeybind">Is this keybind global? Enabled by default.</param>
        /// <param name="Sound">What soundboard sound is this bound to?</param>
        public static Keybind AddKeybind(KeybindTypes KeybindType, List<KeyModifiers> KeyModifiers, VirtualKey Key, Action HandlerFunction, bool GlobalKeybind = true, SoundBoardItem Sound = null)
        {
            Keybind keybind = new Keybind { KeybindType = KeybindType, KeyModifiers = KeyModifiers, Key = Key, Handler = HandlerFunction, GlobalKeybindID = Guid.NewGuid().GetHashCode() };
            // Check if none of the keybind parameters are null
            if (KeybindType == null || KeyModifiers == null || Key == 0 || HandlerFunction == null)
                return keybind;

            Keybinds.Add(keybind);
            if (GlobalKeybind)
            {
                Windows.Win32.Foundation.HWND hWnd = new Windows.Win32.Foundation.HWND(WinRT.Interop.WindowNative.GetWindowHandle(App.m_window));

                // Register global hotkey using PInvoke and pass the callback function
                var success = PInvoke.RegisterHotKey(hWnd, keybind.GlobalKeybindID, (HOT_KEY_MODIFIERS)((uint)KeyModifiers.Sum(x => (int)x)), (uint)Key);
                if (!success)
                    return keybind;

                // Write to the debug console if the keybind was registered successfully
                Debug.WriteLine($"Keybind {GetKeybindText(keybind)} registered with ID {keybind.GlobalKeybindID}");
            }
            return keybind;
        }

        private static void UnregisterGlobalKeybind(Keybind Keybind)
        {
            Windows.Win32.Foundation.HWND hWnd = new Windows.Win32.Foundation.HWND(WinRT.Interop.WindowNative.GetWindowHandle(App.m_window));

            PInvoke.UnregisterHotKey(hWnd, Keybind.Key.GetHashCode());
        }

        /// <summary>
        /// Remove a keybind by the keybind itself
        /// </summary>
        /// <param name="Keybind">The keybind element</param>
        public static void RemoveKeybind(Keybind Keybind)
        {
            UnregisterGlobalKeybind(Keybind);
            Keybinds.Remove(Keybind);
        }

        /// <summary>
        /// Remove a keybind by its index
        /// </summary>
        /// <param name="Index">The keybind index to be removed</param>
        public static void RemoveKeybind(int Index)
        {
            UnregisterGlobalKeybind(Keybinds[Index]);
            Keybinds.RemoveAt(Index);
        }

        /// <summary>
        /// Remove a keybind by the sound it is bound to
        /// </summary>
        /// <param name="Sound">The soundboard sound element the keybind is linked to</param>
        public static void RemoveKeybind(SoundBoardItem Sound)
        {
            Keybind keybind = Keybinds.Find(x => x.Equals(Sound.LinkedKeybind));
            if (keybind.Equals(default(Keybind)))
                return;

            UnregisterGlobalKeybind(keybind);
            Keybinds.Remove(keybind);
        }

        public static void RemoveAllSoundKeybinds()
        {
            Keybinds.RemoveAll(x => x.KeybindType == KeybindTypes.PlaySoundKeybind);
        }

        /// <summary>
        /// Get the keybind text for a keybind
        /// </summary>
        /// <param name="Keybind">The keybind to find the text for</param>
        /// <returns>Kebind Text</returns>
        public static string GetKeybindText(Keybind Keybind)
        {
            string KeybindText = "";
            foreach (var modifier in Keybind.KeyModifiers)
            {
                if (modifier != 0)
                    KeybindText += modifier.ToString() + " + ";
            }
            KeybindText += Keybind.Key.ToString();
            return KeybindText;
        }

        /// <summary>
        /// Get the keybind text for a soundboard sound.
        /// </summary>
        /// <param name="Sound">Soundboard Sound</param>
        /// <remarks>Internally calls <see cref="GetKeybindText(Keybind)">GetKeybindText</see> to find the text.</remarks>
        /// <returns>Keybind Text</returns>
        public static string GetKeybindText(SoundBoardItem Sound)
        {
            Keybind keybind = Keybinds.Find(x => x.Equals(Sound.LinkedKeybind));
            if (keybind.Equals(default(Keybind)))
                return "None";

            return GetKeybindText(keybind);
        }

        public static Keybind? GetKeybindByGlobalID(int GlobalKeybindID)
        {
            return Keybinds.Find(x => x.GlobalKeybindID == GlobalKeybindID);
        }

        /// <summary>
        /// This is a quick function to check if a keybind is pressed. This is used to reduce the amount of writing needed to check if a keybind is pressed.
        /// </summary>
        /// <remarks>This function just redirects to <see cref="CoreWindow.GetKeyState(VirtualKey)"/></remarks>
        /// <param name="key">The key that should be checked.</param>
        /// <returns>Key pressed?</returns>
        private static bool KeyPressed(VirtualKey key) => InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Down);

        /// <summary>
        /// This is called by hyperlink buttons when clicked. 
        /// This locks all app keybinds and inputs until the the keybind is set.
        /// </summary>
        /// <remarks><b>NOTE: </b>This function is an event.</remarks>
        /// <param name="sender">Sender. <b>This should always be the hyperlink keybind button!</b></param>
        /// <param name="e">Unused. Can be null. Required to be there because of the event delegate.</param>
        /// <param name="Keybind"></param>
        public static void KeybindButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Keybind button clicked");

            // Lock all keybinds
            KeybindsEnabled = false;

            // Get sender as HyperlinkButton
            KeybindChangeBtn button = sender as KeybindChangeBtn;


            Keybind tempKeybind = new Keybind();

            // Loop that does not stop the thread. (Async)
            Task.Run(async () =>
            {
                bool ShiftPressed = false;
                bool CtrlPressed = false;
                bool AltPressed = false;
                bool WinPressed = false;

                VirtualKey bindKey = VirtualKey.None;
                bool isKeybindSet = false;

                // Wait for the keybind to be set
                while (!isKeybindSet)
                {
                    ShiftPressed = KeyPressed(VirtualKey.Shift);
                    CtrlPressed = KeyPressed(VirtualKey.Control);
                    AltPressed = KeyPressed(VirtualKey.Menu);
                    WinPressed = KeyPressed(VirtualKey.LeftWindows) || KeyPressed(VirtualKey.RightWindows);
                    // Add text to the button. This requires the UI thread. (DispatcherQueue)
                    await button.DispatcherQueue.EnqueueAsync(() =>
                    {
                        button.Content = $"{(ShiftPressed ? "Shift + " : "")}{(CtrlPressed ? "Ctrl + " : "")}{(AltPressed ? "Alt + " : "")}{(WinPressed ? "Win + " : "")}...";
                    });

                    // Exit the loop if Right Mouse, Left Mouse, or Escape is pressed
                    if (KeyPressed(VirtualKey.RightButton) || KeyPressed(VirtualKey.LeftButton) || KeyPressed(VirtualKey.Escape))
                    {
                        // Unlock all keybinds
                        KeybindsEnabled = true;
                        break;
                    }

                    // Check if any key is pressed
                    for (int key = 1; key <= 255; key++)
                    {
                        if (IllegalKeys.Contains((VirtualKey)key))
                            continue;

                        if (KeyPressed((VirtualKey)key))
                        {
                            bindKey = (VirtualKey)key;
                            isKeybindSet = true;
                            break;
                        }
                    }
                }

                await button.DispatcherQueue.EnqueueAsync(() =>
                {
                    tempKeybind = new Keybind { KeybindType = button.KeybindType, KeyModifiers = new List<KeyModifiers> { (ShiftPressed ? KeyModifiers.Shift : 0), (CtrlPressed ? KeyModifiers.Control : 0), (AltPressed ? KeyModifiers.Alt : 0), (WinPressed ? KeyModifiers.Win : 0) }, Key = bindKey };

                    if (isKeybindSet)
                    {
                        // Check if the keybind is a PlaySoundKeybind
                        if (tempKeybind.KeybindType == KeybindTypes.PlaySoundKeybind)
                        {
                            var item = ((FrameworkElement)sender).DataContext;
                            int index = SoundboardPage.soundBoardItemViewmodel.SoundBoardItems.IndexOf((SoundBoardItem)item);
                            SoundBoardItem soundBoardItem = SoundboardPage.soundBoardItemViewmodel.SoundBoardItems[index];

                            tempKeybind.Handler = SoundboardPage.soundBoardItemViewmodel.SoundBoardItems[index].OnActivated;

                            // Remove old keybind of the same type
                            KeybindsManager.RemoveKeybind(SoundboardPage.soundBoardItemViewmodel.SoundBoardItems[index]);
                            SoundboardPage.soundBoardItemViewmodel.SoundBoardItems[index].SetKeybind(tempKeybind);
                        }
                        else
                        {
                            // Remove old keybind of the same type
                            KeybindsManager.RemoveKeybind(Keybinds.Find(x => x.KeybindType == tempKeybind.KeybindType));

                            // Add the keybind to the keybind list
                            KeybindsManager.AddKeybind(tempKeybind.KeybindType, tempKeybind.KeyModifiers, tempKeybind.Key, tempKeybind.Handler, true);
                        }
                    }
                });
                // Set the keybind
                

                // Update text
                await button.DispatcherQueue.EnqueueAsync(() =>
                {
                    if (isKeybindSet)
                        button.Content = GetKeybindText(tempKeybind);
                    else
                        button.Content = "None";
                });


                // Unlock all keybinds
                KeybindsEnabled = true;
            });
        }
    }
}
