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
            public string DeviceID;
        }

        // Audio Queue list
        public static List<SoundBoardItem> AudioQueue { get; private set; } = new List<SoundBoardItem> { };

        public static List<AudioDevice> GetAudioOutputDevices()
        {
            List<AudioDevice> audioDevices = new List<AudioDevice> { };

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                audioDevices.Add(new AudioDevice { DeviceName = device.FriendlyName, DeviceID = device.ID });
            }

            return audioDevices;
        }

        public static List<AudioDevice> GetAudioInputDevices()
        {
            List<AudioDevice> audioDevices = new List<AudioDevice> { };

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                audioDevices.Add(new AudioDevice { DeviceName = device.FriendlyName, DeviceID = device.ID });
            }

            return audioDevices;
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
            AudioDevice outputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice);
            AudioDevice driverOutputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice);

            // Debug write the device names
            Debug.WriteLine("Output Device: " + outputDevice.DeviceName);
            Debug.WriteLine("Driver Output Device: " + driverOutputDevice.DeviceName);

            // Get the device numbers
            int outputDeviceNumber = GetDeviceNumber(outputDevice.DeviceID);
            int driverOutputDeviceNumber = GetDeviceNumber(driverOutputDevice.DeviceID);

            // Play the sound to both devices at the same time
            using (var outputDeviceWaveOut = new WaveOutEvent())
            using (var driverOutputDeviceWaveOut = new WaveOutEvent())
            {
                // Set the output device
                outputDeviceWaveOut.DeviceNumber = outputDeviceNumber;
                driverOutputDeviceWaveOut.DeviceNumber = driverOutputDeviceNumber;

                // Create separate audio file readers for each device
                using (var outputDeviceAudioFileReader = new AudioFileReader(filePath))
                using (var driverOutputDeviceAudioFileReader = new AudioFileReader(filePath))
                {
                    // Set the audio file readers to the respective devices
                    outputDeviceWaveOut.Init(outputDeviceAudioFileReader);
                    driverOutputDeviceWaveOut.Init(driverOutputDeviceAudioFileReader);

                    // Play the audio
                    outputDeviceWaveOut.Play();
                    driverOutputDeviceWaveOut.Play();

                    // Wait for the audio to finish playing
                    while (outputDeviceWaveOut.PlaybackState == PlaybackState.Playing && driverOutputDeviceWaveOut.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100);
                        if (cancellationToken.IsCancellationRequested)
                        {
                            outputDeviceWaveOut.Stop();
                            driverOutputDeviceWaveOut.Stop();
                            break;
                        }
                    }
                }
            }
        }

        public static async Task PlaySoundboardItem(SoundBoardItem sound, CancellationToken cancellationToken)
        {
            // This function plays audio over the selected output device AND the hardware output device at the same time

            // Get the audio file path
            string filePath = sound.PhysicalFilePath;

            // Play the sound to both devices at the same time
            Task outputTask = Task.Run(() => PlaySound(filePath, SoundboardAudioType.LocalFile, cancellationToken));
            Task driverOutputTask = Task.Run(() => PlaySound(filePath, SoundboardAudioType.LocalFile, cancellationToken));
            await driverOutputTask;
            await outputTask;
        }

        private static int GetDeviceNumber(string deviceID)
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDeviceCollection devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].ID == deviceID)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
