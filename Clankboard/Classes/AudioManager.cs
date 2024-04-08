using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using Clankboard.Classes;
using System.Threading;

namespace Clankboard
{
    public static class AudioManager
    {
        public struct AudioDevice
        {
            public string DeviceName;
            public Guid DeviceGUID;
            public int DeviceNumber;
        }

        // Audio Queue list
        public static List<SoundBoardItem> AudioQueue { get; private set; } = new List<SoundBoardItem> { };

        public static List<AudioDevice> AudioOutputDevices { get; private set; } = new List<AudioDevice> { };
        public static List<AudioDevice> AudioInputDevices { get; private set; } = new List<AudioDevice> { };

        // Default audio device
        public readonly static AudioDevice DefaultAudioOutputDevice = new AudioDevice { DeviceName = "Default Output Device", DeviceGUID = Guid.Empty, DeviceNumber = -101 };
        public readonly static AudioDevice DefaultAudioInputDevice = new AudioDevice { DeviceName = "Default Microphone", DeviceGUID = Guid.Empty, DeviceNumber = -101 };

        public static void UpdateAudioOutputDevices()
        {
            AudioOutputDevices.Clear();
            AudioOutputDevices.Add(DefaultAudioOutputDevice);

            // Enumerate using DirectSound
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                AudioOutputDevices.Add(new AudioDevice { DeviceName = caps.ProductName, DeviceGUID = caps.ProductGuid, DeviceNumber = n });
            }
        }

        public static void UpdateAudioInputDevices()
        {
            AudioInputDevices.Clear();
            AudioInputDevices.Add(DefaultAudioInputDevice);

            // Enumerate using DirectSound
            for (int n = -1; n < WaveIn.DeviceCount; n++)
            {
                var caps = WaveIn.GetCapabilities(n);
                AudioInputDevices.Add(new AudioDevice { DeviceName = caps.ProductName, DeviceGUID = caps.ProductGuid, DeviceNumber = n });
            }
        }

        public enum SoundboardAudioType
        {
            LocalFile,
            CloudFile,
            EmbeddedInSoundBoardFile
        }

        // Play a sound from a file
        private static async void PlaySound(string filePath, SoundboardAudioType audioType, CancellationToken cancellationToken)
        {
            // Get the audio devices
            //AudioDevice outputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice);
            //AudioDevice driverOutputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice);

            //// Debug write the device names
            //Debug.WriteLine("Output Device: " + outputDevice.DeviceName);
            //Debug.WriteLine("Driver Output Device: " + driverOutputDevice.DeviceName);

            //int outputDeviceNumber = GetDeviceNumber(outputDevice.DeviceID);
            //int driverOutputDeviceNumber = GetDeviceNumber(driverOutputDevice.DeviceID);

            //var waveOut = new WaveOut();

            throw new NotImplementedException("This function is DEPRECATED/UNUSED! Function is currently only a stub.");

        }

        private static void PlayAudioFileInDevice(AudioDevice device, string filePath, CancellationToken cancellation)
        {
            // Play the audio file in the selected device
            var waveOut = new WaveOut();
            var audioFile = new AudioFileReader(filePath);
            waveOut.DeviceNumber = device.DeviceNumber;
            waveOut.Init(audioFile);
            waveOut.Play();
            // Loop until the audio file is finished playing OR the cancellation token is cancelled
            while (waveOut.PlaybackState == PlaybackState.Playing && !cancellation.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            // Stop and dispose of the audio file
            waveOut.Stop();
            waveOut.Dispose();
            audioFile.Dispose();
        }

        /// <summary>
        /// Play a soundboard item on the currently set output devices in <see cref="SettingsManager"/>.
        /// </summary>
        /// <param name="sound">The soundboardItem</param>
        /// <param name="cancellationToken">The cancellationToken. This is required for sound cancellation.</param>
        /// <returns></returns>
        public static async Task PlaySoundboardItem(SoundBoardItem sound, CancellationToken cancellationToken)
        {
            GetDeviceNumber("");
            // Get the audio devices
            AudioDevice outputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice);
            AudioDevice driverOutputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice);

            // Debug write the device names
            Debug.WriteLine("Output Device: " + outputDevice.DeviceName);
            Debug.WriteLine("Driver Output Device: " + driverOutputDevice.DeviceName);

            // Grab devices from SettingsManager
            AudioDevice outputDeviceNumber = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice);
            AudioDevice driverOutputDeviceNumber = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice);

            // Call PlayAudioFileInDevice for each device
            Task.Run(() => PlayAudioFileInDevice(outputDeviceNumber, sound.PhysicalFilePath, cancellationToken));
            Task.Run(() => PlayAudioFileInDevice(driverOutputDeviceNumber, sound.PhysicalFilePath, cancellationToken));
        }

        private static int GetDeviceNumber(string deviceID)
        {
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Debug.WriteLine($"{n}: {caps.ProductName}; GUID: {caps.ProductGuid}; GIVEN DEVICEID: {deviceID}");
                // Debug write the device Number
            }

            // Return the device number by DeviceID

            return -1;

            //MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            //MMDeviceCollection devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            //for (int i = 0; i < devices.Count; i++)
            //{
            //    if (devices[i].ID == deviceID)
            //    {
            //        // Debug write the device ID
            //        Debug.WriteLine("Device ID: " + deviceID);
            //        // Debug write the device number
            //        Debug.WriteLine("Device Number: " + (i));

            //        // Debug print the actual device name for the device number that was found
            //        Debug.WriteLine("Real Device Name: " + devices[i].FriendlyName);
            //        return i;
            //    }
            //}
            //return -1;
        }
    }
}
