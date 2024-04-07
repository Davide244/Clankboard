using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
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
            public SoundBoardItem Sound;                   // The soundboard item it is bound to (if it is a PlaySoundKeybind)
            public Action Handler;                         // The function that runs when the keybind is pressed, takes a parameter of SoundBoardItem that can be null
        }

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
            Keybind keybind = new Keybind { KeybindType = KeybindType, KeyModifiers = KeyModifiers, Key = Key, Sound = Sound, Handler = HandlerFunction, GlobalKeybindID = Key.GetHashCode() };
            Keybinds.Add(keybind);
            if (GlobalKeybind)
            {
                Windows.Win32.Foundation.HWND hWnd = new Windows.Win32.Foundation.HWND(WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App)?.m_window as MainWindow));

                // Register global hotkey using PInvoke and pass the callback function
                var success = PInvoke.RegisterHotKey(hWnd, keybind.GlobalKeybindID, (HOT_KEY_MODIFIERS)((uint)KeyModifiers.Sum(x => (int)x)), (uint)Key);
                if (!success)
                    return keybind;

                // Write to the debug console if the keybind was registered successfully
                Debug.WriteLine($"Keybind {GetKeybindText(keybind)} registered with ID {keybind.GlobalKeybindID}");
            }
            return keybind;
        }

        private static void UregisterGlobalKeybind(Keybind Keybind)
        {
            Windows.Win32.Foundation.HWND hWnd = new Windows.Win32.Foundation.HWND(WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App)?.m_window as MainWindow));

            PInvoke.UnregisterHotKey(hWnd, Keybind.Key.GetHashCode());
        }

        /// <summary>
        /// Remove a keybind by the keybind itself
        /// </summary>
        /// <param name="Keybind">The keybind element</param>
        public static void RemoveKeybind(Keybind Keybind)
        {
            UregisterGlobalKeybind(Keybind);
            Keybinds.Remove(Keybind);
        }

        /// <summary>
        /// Remove a keybind by its index
        /// </summary>
        /// <param name="Index">The keybind index to be removed</param>
        public static void RemoveKeybind(int Index)
        {
            UregisterGlobalKeybind(Keybinds[Index]);
            Keybinds.RemoveAt(Index);
        }

        /// <summary>
        /// Remove a keybind by the sound it is bound to
        /// </summary>
        /// <param name="Sound">The soundboard sound element the keybind is linked to</param>
        public static void RemoveKeybind(SoundBoardItem Sound)
        {
            UregisterGlobalKeybind(Keybinds.Find(x => x.Sound == Sound));
            Keybinds.RemoveAll(x => x.Sound == Sound);
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
            Keybind keybind = Keybinds.Find(x => x.Sound == Sound);
            if (keybind.Equals(default(Keybind)))
                return "None";

            return GetKeybindText(keybind);
        }

        public static Keybind? GetKeybindByGlobalID(int GlobalKeybindID)
        {
            return Keybinds.Find(x => x.GlobalKeybindID == GlobalKeybindID);
        }
    }
}
