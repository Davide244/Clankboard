using Microsoft.UI.Xaml;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

namespace Clankboard.AudioSystem
{
    public class ClankAudioDeviceManager
    {
        public List<MMDevice> availableOutputDevices = new List<MMDevice>();
        public List<MMDevice> availableInputDevices = new List<MMDevice>();

        /// <summary>
        /// The key is the mmres icon number, the value is the icon glyph.
        /// </summary>
        private static readonly Dictionary<int, mmresIconDeviceTypeInformation> mmresWinui3IconAlternatives = new Dictionary<int, mmresIconDeviceTypeInformation>()
        {
            { -1, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE994", iconName = "Unknown", iconFontFamily = null } },
            { 3004, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE994", iconName = "Speaker", iconFontFamily = null } },

            { 3010, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE7F5", iconName = "Speaker Box", iconFontFamily = null } },
            { 3019, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE7F5", iconName = "Speaker Box", iconFontFamily = null } },
            { 3030, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE7F5", iconName = "Speaker Box", iconFontFamily = null } },
            { 3050, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE7F5", iconName = "Speaker Box", iconFontFamily = null } },

            { 3011, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE7F6", iconName = "Headphones", iconFontFamily = null } },
            { 3031, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE7F6", iconName = "Headphones", iconFontFamily = null } },
            { 3051, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE7F6", iconName = "Headphones", iconFontFamily = null } },

            { 3012, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE95F", iconName = "Audio Cable", iconFontFamily = null } },

            { 3013, new mmresIconDeviceTypeInformation() { iconGlyph = "B", iconName = "Audio Reciever", iconFontFamily = Application.Current.Resources["ClankboardSymbolFont"].ToString() } },

            { 3014, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE720", iconName = "Microphone", iconFontFamily = null } },
            { 3021, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE720", iconName = "Microphone", iconFontFamily = null } },

            { 3015, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE95B", iconName = "Headset", iconFontFamily = null } },

            { 3016, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE717", iconName = "Telephone", iconFontFamily = null } },

            { 3017, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE7F4", iconName = "Monitor", iconFontFamily = null } },

            { 3018, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE964", iconName = "Sound Card", iconFontFamily = null } },

            { 3020, new mmresIconDeviceTypeInformation() { iconGlyph = "\uE960", iconName = "Webcam", iconFontFamily = null } }
        };

        public void UpdateOutputDevices(DeviceState filter = DeviceState.Active) 
        {
            availableOutputDevices.Clear();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, filter))
            {
                try
                {
                    Debug.WriteLine("Found output device: " + device.FriendlyName + " || " + device.ID + " || " + device.State + " || " + device.IconPath);
                    availableOutputDevices.Add(device);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error while adding output device: " + e.Message); // Fix for app crash on some computers. No idea why it throws an exception.
                }
            }
        }

        public void UpdateInputDevices(DeviceState filter = DeviceState.Active)
        {
            availableInputDevices.Clear();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, filter))
            {
                try
                {
                    Debug.WriteLine("Found input device: " + device.FriendlyName + " || " + device.ID + " || " + device.State + " || " + device.DeviceFriendlyName);
                    availableInputDevices.Add(device);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error while adding input device: " + e.Message); // Fix for app crash on some computers. No idea why it throws an exception.
                }
            }
        }

        public int GetInputDeviceSampleRate(MMDevice device)
        {
            return device.AudioClient.MixFormat.SampleRate;
        }

        /// <summary>
        /// Returns the device type of the device.
        /// Used to display the correct icon for the device type in selector dropdowns.
        /// </summary>
        /// <param name="device">The device to be checked.</param>
        /// <returns>mmres Icon Info.</returns>
        public mmresIconDeviceTypeInformation GetDeviceTypeIconInformation(MMDevice device)
        {
            if (device.IconPath == null)
            {
                return mmresWinui3IconAlternatives[-1];
            }

            string[] iconPathParts = device.IconPath.Split(',');

            if (iconPathParts.Length < 2)
            {
                return mmresWinui3IconAlternatives[-1];
            }

            int iconNumber = -1;

            if (!int.TryParse(iconPathParts[1], out iconNumber))
            {
                return mmresWinui3IconAlternatives[-1];
            }

            if (mmresWinui3IconAlternatives.ContainsKey(Math.Abs(iconNumber)))
            {
                return mmresWinui3IconAlternatives[Math.Abs(iconNumber)];
            }
            else
            {
                return mmresWinui3IconAlternatives[-1];
            }
        }
    }

    public struct mmresIconDeviceTypeInformation
    {
        public string iconGlyph;
        public string iconName;
        public string iconFontFamily;
    }

    /// <summary>
    /// This enum is used to determine if a device is a headset, speaker, microphone, etc.
    /// For this, it uses the icon path of the device.
    /// </summary>
    public enum mmresAudioDeviceType
    {
        Speaker,
        SpeakerBox,
        Headphones,
        AudioCable,
        AudioReceiver,
        Microphone,
        Headset,
        Telephone,
        Monitor,
        SoundCard,
        Webcam,
        Unknown
    }
}
