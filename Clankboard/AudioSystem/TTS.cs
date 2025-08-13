using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Media.SpeechSynthesis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Clankboard.AudioSystem;

/// <summary>
///     Represantation of installed synthetic voice
/// </summary>
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

public partial class TTSVoices : ObservableObject
{
    // Singleton pattern
    private static TTSVoices instance;

    /// <summary>
    ///     List of installed voices.
    /// </summary>
    [ObservableProperty] public ObservableCollection<TTSVoice> _installedVoices;

    public static TTSVoices Instance
    {
        get
        {
            if (instance == null) instance = new TTSVoices();
            return instance;
        }
    }
}

public static class TTSHelper
{
    public static async void UpdateInstalledVoices()
    {
        if (TTSVoices.Instance.InstalledVoices == null)
            TTSVoices.Instance.InstalledVoices = new ObservableCollection<TTSVoice>();
        else
            TTSVoices.Instance.InstalledVoices.Clear();

        var voices = SpeechSynthesizer.AllVoices;
        foreach (var voice in voices)
        {
            Debug.WriteLine("-------------------------");
            Debug.WriteLine("Voice Name: " + voice.DisplayName);
            Debug.WriteLine("Voice Language: " + voice.Language);
            Debug.WriteLine("Voice Gender: " + (voice.Gender == VoiceGender.Male ? "Male" : "Female"));
            Debug.WriteLine("Voice Description: " + voice.Description);
            Debug.WriteLine("Voice ID: " + voice.Id);
            Debug.WriteLine("-------------------------");
            TTSVoices.Instance.InstalledVoices.Add(new TTSVoice(voice.DisplayName, voice.Language, voice.Gender,
                voice.Description, voice.Id));
        }
    }
}