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
using static Clankboard.AudioManager;
using System.ComponentModel;

namespace Clankboard
{
    public static class AudioManager
    {
        #region Audio Device & Audio Playback
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

            AudioFileReader audioFile = null;

            try
            {
                audioFile = new AudioFileReader(filePath);
            }
            catch (Exception e)
            {
                // if FileNotFoundException, give error message and return
                if (e is System.IO.FileNotFoundException)
                {
                    Debug.WriteLine("File not found: " + filePath);
                    return;
                }

                return;
            }
            

            // Check if the device is the default device. If no, set the device number. If yes, skip.
            if (device.DeviceNumber != DefaultAudioInputDevice.DeviceNumber && device.DeviceNumber != DefaultAudioOutputDevice.DeviceNumber)
            {
                waveOut.DeviceNumber = device.DeviceNumber;
            }

            //waveOut.DeviceNumber = device.DeviceNumber;
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

        #endregion
    }

    /// <summary>
    /// Concrete class that houses code for the microphone mixing functionality.
    /// It works by getting the microphone input and outputting it to the selected audio output device.
    /// </summary>
    public class MicMixer
    {
        private WaveIn waveIn;
        private WaveOut waveOut;

        public AudioDevice InputDevice 
        {
            get { return AudioInputDevices.FirstOrDefault(x=>x.DeviceNumber == waveIn.DeviceNumber); }
            set 
            {
                waveIn.DeviceNumber = value.DeviceNumber;
            }
        }
        public AudioDevice OutputDevice
        {
            get { return AudioOutputDevices.FirstOrDefault(x => x.DeviceNumber == waveOut.DeviceNumber); }
            set
            {
                waveOut.DeviceNumber = value.DeviceNumber;
            }
        }

        private BufferedWaveProvider bufferedWaveProvider;

        private bool IsEnabled = false;

        /// <summary>
        /// Constructor for the MicMixer class. Calls the InitializeMicMix private method.
        /// </summary>
        public MicMixer(AudioDevice inputDevice, AudioDevice outputDevice)
        {
            // Create the waveIn and waveOut instances
            waveIn = new WaveIn();
            waveOut = new WaveOut();

            // Device input (Set to 0 if negative)
            waveIn.DeviceNumber = inputDevice.DeviceNumber < 0 ? 0 : inputDevice.DeviceNumber;
            waveOut.DeviceNumber = outputDevice.DeviceNumber < 0 ? 0 : outputDevice.DeviceNumber;

            bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
            bufferedWaveProvider.DiscardOnBufferOverflow = true;

            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;

            waveOut.Init(bufferedWaveProvider);
        }

        /// <summary>
        /// Enabled the microphone mixing functionality. This will start playing the microphone audio to the selected audio output device.
        /// </summary>
        public void Enable()
        {
            if (!IsEnabled)
            {
                waveIn.StartRecording();
                waveOut.Play();
                IsEnabled = true;
            }
        }

        /// <summary>
        /// Disabled microphone mixing. This will stop playing the microphone audio to the selected audio output device.
        /// </summary>
        public void Disable()
        {
            if (IsEnabled)
            {
                waveIn.StopRecording();
                //waveOut.Pause();
                IsEnabled = false;
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // Play the audio data from the microphone to the WaveOut instance
            bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Dispose the waveIn and waveOut instances when playback is stopped
            //waveIn?.Dispose();
            waveOut?.Dispose();
        }
    }
}
