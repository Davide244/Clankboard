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
using System.ComponentModel;
using NAudio.Wave.SampleProviders;

namespace Clankboard
{
    public class AudioManager
    {
        private WaveIn MicrophoneWaveIn;
        private WaveOut VAC_WaveOut;
        private WaveOut Local_WaveOut;

        private BufferedWaveProvider VAC_BufferedWaveProvider;
        private BufferedWaveProvider Local_BufferedWaveProvider;

        private MixingSampleProvider VAC_Mixer;
        private MixingSampleProvider Local_Mixer;

        #region Audio Device & Audio Playback
        public struct AudioDevice
        {
            public string DeviceName;
            public Guid DeviceGUID;
            public int DeviceNumber;
        }

        public enum SoundboardAudioType
        {
            LocalFile,
            CloudFile,
            EmbeddedInSoundBoardFile
        }

        // Audio Queue list
        public static List<SoundBoardItem> PlayingAudios { get; private set; } = new List<SoundBoardItem> { };
        public static readonly int MaxPlayingAudios = 20;

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

        private void PlayAudioFile(MixingSampleProvider audioMixer, string filePath, CancellationToken cancellation)
        {
            AudioFileReader audioFile;
            MediaFoundationResampler resampledAudio;

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

            if (audioFile == null)
                return;

            // Resample the audio to the mixer's sample rate
            resampledAudio = new MediaFoundationResampler(audioFile, audioMixer.WaveFormat);
            resampledAudio.ResamplerQuality = 40;
            audioMixer.AddMixerInput(resampledAudio.ToSampleProvider());

            int AudioLength = (int)(audioFile.TotalTime.TotalMilliseconds);
            Thread.Sleep(AudioLength);

            resampledAudio.Dispose();
            //audioMixer.RemoveMixerInput(resampledAudio.ToSampleProvider());
            audioFile.Dispose();
        }

        /// <summary>
        /// Play a soundboard item on the currently set output devices in <see cref="SettingsManager"/>.
        /// </summary>
        /// <param name="sound">The soundboardItem</param>
        /// <param name="cancellationToken">The cancellationToken. This is required for sound cancellation.</param>
        /// <returns></returns>
        public async Task PlaySoundboardItem(SoundBoardItem sound, CancellationToken cancellationToken)
        {
            // Check how many sounds are playing
            if (PlayingAudios.Count >= MaxPlayingAudios && PlayingAudios.Count > 0)
            {
                Debug.WriteLine("Too many sounds playing. Stopping sound: " + sound.SoundName +"@" + sound.PhysicalFilePath);

                // Stop the sound at the 0th element
                PlayingAudios[0].StopSound();
                PlayingAudios.RemoveAt(0);
            }

            // Add the sound to the end of the list
            PlayingAudios.Add(sound);

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
            Task.Run(() => PlayAudioFile(Local_Mixer, sound.PhysicalFilePath, cancellationToken));
            Task.Run(() => PlayAudioFile(VAC_Mixer, sound.PhysicalFilePath, cancellationToken));
        }

        #endregion

        #region Microphone Mixing

        /// <summary>
        /// Constructor for the AudioManager. Initializes the audio devices for mixing.
        /// </summary>
        public AudioManager()
        {
            // Initialize the audio devices.

            // The audio flow is as follows:
            // Microphone audio:
            //      Input Device --> BufferedWaveProvider --> MixingSampleProvider --> WaveOut
            // Soundboard audio:                                     ↑
            //      AudioFileReader ———————————————————————————————.⅃

            MicrophoneWaveIn = new WaveIn();
            VAC_WaveOut = new WaveOut();
            Local_WaveOut = new WaveOut();

            MicrophoneWaveIn.WaveFormat = new(44100/*Hz*/, 32/*bit*/, 2);

            VAC_BufferedWaveProvider = new BufferedWaveProvider(MicrophoneWaveIn.WaveFormat);
            Local_BufferedWaveProvider = new BufferedWaveProvider(MicrophoneWaveIn.WaveFormat);

            VAC_BufferedWaveProvider.DiscardOnBufferOverflow = true;
            Local_BufferedWaveProvider.DiscardOnBufferOverflow = true;

            VAC_Mixer = new MixingSampleProvider(new ISampleProvider[] { VAC_BufferedWaveProvider.ToSampleProvider() });
            Local_Mixer = new MixingSampleProvider(new ISampleProvider[] { Local_BufferedWaveProvider.ToSampleProvider() });

            // Set the audio data available event
            MicrophoneWaveIn.DataAvailable += MicrophoneWaveIn_DataAvailable;

            // Set the output & input devices
            AudioDevice LocalOutputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.LocalOutputDevice);
            AudioDevice VACOutputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.VACOutputDevice);
            AudioDevice InputDevice = SettingsManager.GetSetting<AudioDevice>(SettingsManager.SettingTypes.InputDevice);

            VAC_WaveOut.DeviceNumber = VACOutputDevice.DeviceNumber;
            Local_WaveOut.DeviceNumber = LocalOutputDevice.DeviceNumber;
            MicrophoneWaveIn.DeviceNumber = InputDevice.DeviceNumber;

            VAC_WaveOut.Init(VAC_Mixer);
            Local_WaveOut.Init(Local_Mixer);
        }

        public void StartMicrophone()
        {
            MicrophoneWaveIn.StartRecording();
            VAC_WaveOut.Play();
            Local_WaveOut.Play();
        }

        // Event handler for when the microphone captures audio
        private void MicrophoneWaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // Write the audio data to the BufferedWaveProvider
            VAC_BufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            Local_BufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        #endregion
    }
}
