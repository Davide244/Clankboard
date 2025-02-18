using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.AudioSystem
{
    /// <summary>
    /// Audio routing class.
    /// Uses NAudio to route audio through the different systems inside of Clankboard.
    /// </summary>
    public class AudioRouting
    {

        // 3 NAudio mixer instances for the 2 different audio systems.
        private NAudio.Wave.WaveMixerStream32 soundboardAudioMixer; // Mixes soundboard audio together into one stream.
        private NAudio.Wave.WaveMixerStream32 mainOutputMixer;      // Mixes mixed soundboard audio with the microphonen for the main output.
    }
}
