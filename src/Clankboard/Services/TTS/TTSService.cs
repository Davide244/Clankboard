using Clankboard.Services.TTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;

namespace Clankboard.Services.TTS
{
    public interface ITTSService
    {
        Task<string> GenerateTTSAudioAsync(string text, string voice, int speed, string outputFilePath);
        Task<List<TTSVoice>> GetTTSVoiceList();
    }

    public class TTSService : ITTSService
    {
        public Task<string> GenerateTTSAudioAsync(string text, string voice, int speed, string outputFilePath)
        {
            throw new NotImplementedException();
        }

        public Task<List<TTSVoice>> GetTTSVoiceList()
        {
            var voices = Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices;
            var ttsVoices = new List<TTSVoice>(voices.Count);
            foreach (var voice in voices)
            {
                var ttsVoice = new TTSVoice(voice.DisplayName, voice.Language, voice.Gender, voice.Description, voice.Id);
                ttsVoices.Add(ttsVoice);
            }
            return Task.FromResult(ttsVoices);
        }
    }
}
