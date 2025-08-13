using NAudio.Wave;

namespace Clankboard.AudioSystem;

/// <summary>
///     Audio routing class.
///     Uses NAudio to route audio through the different systems inside of Clankboard.
/// </summary>
public class AudioRouting
{
    /*
     * FLOW OF AUDIO FROM SOUNDBOARD
     *
     *  SOUND ------> RESAMPLER -----------------------------> MIXER -----> Mixer -----> Main Output
     *                    |----------> VoiceBox modulator -------^            |--------> Local Output
     *
     */

    public const int
        SAMPLE_RATE =
            44100; // Sample rate for the audio system. This only applies, if no microphone is used. If a microphone is used, the sample rate is determined by the microphone.

    private WaveMixerStream32 mainOutputMixer; // Mixes mixed soundboard audio with the microphonen for the main output.


    // 2 NAudio mixer instances for the 2 different audio systems.
    private WaveMixerStream32 soundboardAudioMixer; // Mixes soundboard audio together into one stream.

    public AudioRouting()
    {
        soundboardAudioMixer = new WaveMixerStream32();
        mainOutputMixer = new WaveMixerStream32();
    }
}