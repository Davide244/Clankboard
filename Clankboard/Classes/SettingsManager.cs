using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email.DataProvider;
using Windows.Devices.Geolocation;
using static Clankboard.AudioManager;

namespace Clankboard.Classes
{
    public static class SettingsManager
    {
        // Add a new enum to store the settings and their actual types
        public enum SettingTypes
        {
            #region Audio Device Volume Settings
            /// <summary>
            /// Local output volume. This is the volume outputted to the local device.
            /// </summary>
            [SettingType(typeof(int))]
            LocalOutputVolume,

            /// <summary>
            /// VAC output volume. This is the volume outputted to the virtual audio cable.
            /// </summary>
            [SettingType(typeof(int))]
            VACOutputVolume,

            /// <summary>
            /// Microphone volume. This is the volume of the microphone.
            /// </summary>
            [SettingType(typeof(int))]
            MicrophoneVolume,
            #endregion

            #region Audio Device Settings
            [SettingType(typeof(AudioManager.AudioDevice))]
            LocalOutputDevice,

            [SettingType(typeof(AudioManager.AudioDevice))]
            VACOutputDevice,

            [SettingType(typeof(AudioManager.AudioDevice))]
            InputDevice,
            #endregion

            /// <summary>
            /// Output microphone to VAC. This is whether the microphone output is sent to the virtual audio cable.
            /// </summary>
            /// <remarks><b>AKA: </b>Audio mixer enabled for microphone.</remarks>
            [SettingType(typeof(bool))]
            OutputMicrophoneToVAC,

            /// <summary>
            /// Hear yourself enabled. This is whether the user can hear themselves through the microphone.
            /// </summary>
            [SettingType(typeof(bool))]
            HearYourselfEnabled,

            #region Soundboard Settings
            [SettingType(typeof(bool))]
            AudioStackingEnabled,

            [SettingType(typeof(KeybindsManager.Keybind))]
            StopAllSoundsKeybind,

            [SettingType(typeof(bool))]
            UseCompactSoundboardList,
            #endregion
        }

        /// <summary>
        /// This is a readonly default audio device that is used when the audio device is not set.
        /// </summary>
        /// <remarks> If this is set, no parameters are passed to set the DeviceNumber while playing and relaying audio. </remarks>
        //public static readonly AudioDevice DefaultAudioDevice = new AudioDevice { DeviceGUID = Guid.Empty, DeviceName = "DEFAULT", DeviceNumber = -101 };

        private static Dictionary<SettingTypes, object> settings = new Dictionary<SettingTypes, object>();
        // Default values for the settings
        private static Dictionary<SettingTypes, object> defaultSettings = new Dictionary<SettingTypes, object>
        {
            { SettingTypes.LocalOutputVolume, 100 },
            { SettingTypes.VACOutputVolume, 100 },
            { SettingTypes.MicrophoneVolume, 100 },
            { SettingTypes.LocalOutputDevice, DefaultAudioOutputDevice },
            { SettingTypes.VACOutputDevice, DefaultAudioOutputDevice },
            { SettingTypes.InputDevice, DefaultAudioInputDevice },
            { SettingTypes.OutputMicrophoneToVAC, true },
            { SettingTypes.AudioStackingEnabled, false },
            { SettingTypes.StopAllSoundsKeybind, null },
            { SettingTypes.UseCompactSoundboardList, false },
            { SettingTypes.HearYourselfEnabled, false },
        };

        public static void SetSetting<T>(SettingTypes name, T value)
        {
            T valueTemp;
            if (typeof(T) != GetSettingType(name))
            {
                // Change the type of the value to the type of the setting
                if (value == null)
                {
                    settings[name] = null;
                    return;
                }

                valueTemp = (T)Convert.ChangeType(value, GetSettingType(name));
                settings[name] = valueTemp;
                return;

                //throw new InvalidCastException($"Setting '{name}' is not of type '{typeof(T)}'.");
            }

            settings[name] = value;

            // Print to debug
            //Debug.WriteLine($"Setting '{name}' set to '{value}'");

            // Save settings to file
            FileManagers.SettingsFileManager.Instance.SaveFile();
        }

        public static T GetSetting<T>(SettingTypes name)
        {
            if (settings.ContainsKey(name))
            {
                return (T)settings[name];
            }
            else
            {
                //throw new KeyNotFoundException($"Setting '{name}' not found.");
                // return empty value
                return default(T);
            }
        }

        // return settings as list
        public static List<KeyValuePair<SettingTypes, object>> GetSettings()
        {
            return settings.ToList();
        }

        public static bool SettingExists(SettingTypes name)
        {
            return settings.ContainsKey(name);
        }

        public static Type GetSettingType(SettingTypes name)
        {
            var field = typeof(SettingTypes).GetField(name.ToString());
            var attribute = field.GetCustomAttributes(typeof(SettingTypeAttribute), false).FirstOrDefault() as SettingTypeAttribute;
            return attribute?.Type;
        }

        public static object GetDefaultSettingValue(SettingTypes setting)
        {
            if (defaultSettings.ContainsKey(setting))
            {
                return defaultSettings[setting];
            }
            else
            {
                throw new KeyNotFoundException($"Default setting value for '{setting}' not found.");
            }
        }

        /// <summary>
        /// This function can do one thing:
        /// Reset all settings to their default values
        /// </summary>
        public static void InitializeSettings()
        {
            foreach (SettingTypes setting in Enum.GetValues(typeof(SettingTypes)))
            {
                settings[setting] = GetDefaultSettingValue(setting);
            }
        }

        /// <summary>
        /// Attribute to store the type of the setting
        /// </summary>
        public class SettingTypeAttribute : Attribute
        {
            public Type Type { get; }

            public SettingTypeAttribute(Type type)
            {
                Type = type;
            }
        }
    }
}
