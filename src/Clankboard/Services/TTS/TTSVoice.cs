using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Globalization;
using Windows.Media.SpeechSynthesis;

namespace Clankboard.Services.TTS
{
    public partial class TTSVoice : ObservableObject
    {
        [ObservableProperty] public string _description;
        [ObservableProperty] public string _gender;
        [ObservableProperty] public string _iD;
        [ObservableProperty] public string _language;
        [ObservableProperty] public string _name;

        private VoiceGender voiceGender;

        public TTSVoice(string name, string language, VoiceGender gender, string description, string iD)
        {
            Name = name;
            Language = language;
            voiceGender = gender;
            Gender = gender == VoiceGender.Male ? "Male" : "Female";
            Description = description;
            ID = iD;
        }
    }
}
