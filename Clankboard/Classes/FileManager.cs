using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using Clankboard.Classes;
using System.IO.Compression;
using Windows.System;

namespace Clankboard.Classes.FileManagers
{
    public struct ClankFile
    {
        public string Name { get; set; }
        public string FolderPath { get; set; }
    }

   

    /// <summary>
    /// This class handles soundboard file loading and saving.
    /// </summary>
    public class SoundboardFileManager
    {
        private readonly int ClankFileVersion = 1;

        #region Singleton vars
        private static SoundboardFileManager instance = null;
        private static readonly object padlock = new object();
        #endregion

        #region Soundboard File Structure (Enums and Structs)
        public enum SoundboardFileEntryType
        {
            LocalFile,
            DownloadedFile
        }

        public struct SoundboardFileEntry
        {
            public SoundboardFileEntryType Type { get; set; }
            public string Name { get; set; }
            public bool Embedded { get; set; }
            public string Path { get; set; }
            public KeybindsManager.Keybind Keybind { get; set; }
        }

        public struct SoundboardFile
        {
            public int ClankFileVersion { get; set; }
            public string Name { get; set; }
            public List<SoundboardFileEntry> Sounds { get; set; }
        }
        #endregion

        /// <summary>
        /// Current soundboard file. This is the file that is currently loaded / used.
        /// </summary>
        public ClankFile CurrentSoundboardFile { get; private set; }

        public void LoadFile(string path)
        {
            // Clear the soundboard
            SoundboardPage.soundBoardItemViewmodel.SoundBoardItems.Clear();

            KeybindsManager.RemoveAllSoundKeybinds();

            // Extract the zip file to the temp folder and read the clankfile.json
            string tempFolder = Path.Combine(Path.GetTempPath(), "Clankboard", "TempSoundboardFiles", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            // Extract the zip file
            ZipFile.ExtractToDirectory(path, tempFolder);

            // Read the clankfile.json
            string JSONPath = Path.Combine(tempFolder, "clankfile.json");
            string JSON = File.ReadAllText(JSONPath);

            // Deserialize the JSON
            SoundboardFile soundboardFile = JsonConvert.DeserializeObject<SoundboardFile>(JSON);
            SoundBoardItem CurrentItem;
            KeybindsManager.Keybind tempKeybind = new KeybindsManager.Keybind();

            // Loop through all SoundboardFileEntries and add them to the soundboard
            foreach (SoundboardFileEntry entry in soundboardFile.Sounds)
            {
                CurrentItem = new SoundBoardItem(entry.Name, entry.Path, "\uE8A5", true, "Test", false, true, false, 100, null, null, entry.Path);

                if (entry.Embedded)
                {
                    CurrentItem.PhysicalFilePath = Path.Combine(tempFolder, entry.Path.TrimStart('\\'));
                }

                tempKeybind = entry.Keybind;
                tempKeybind.Handler = CurrentItem.OnActivated;

                // If the keybind equals the default of the Keybind struct, it means that the keybind is not set.
                if (tempKeybind.Key != VirtualKey.None)
                {
                    CurrentItem.SetKeybind(tempKeybind);
                }

                // Add the item to the soundboard
                SoundboardPage.soundBoardItemViewmodel.SoundBoardItems.Add(CurrentItem);
            }
        }

        public void SaveFile(string path, string name, bool EmbedOnlineFiles = false, bool EmbedLocalFiles = true)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "Clankboard", "TempSoundboardFiles", name + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            if (EmbedOnlineFiles)
                Directory.CreateDirectory(Path.Combine(tempFolder, "OnlineSounds"));
            if (EmbedLocalFiles)
                Directory.CreateDirectory(Path.Combine(tempFolder, "LocalSounds"));

            string JSONPath = Path.Combine(tempFolder, "clankfile.json");

            List<SoundBoardItem> SoundboardItems = SoundboardPage.soundBoardItemViewmodel.GetSoundboardItems();
            List<SoundboardFileEntry> SoundboardFileEntries = new List<SoundboardFileEntry>();
            KeybindsManager.Keybind tempKeybind = new KeybindsManager.Keybind();

            foreach (SoundBoardItem item in SoundboardItems)
            {
                SoundboardFileEntry entry = new SoundboardFileEntry
                {
                    Name = item.SoundName,
                    Path = item.PhysicalFilePath,
                    Type = item.ItemType,
                    Embedded = (item.ItemType == SoundboardFileEntryType.LocalFile && EmbedLocalFiles) ||
                               (item.ItemType == SoundboardFileEntryType.DownloadedFile && EmbedOnlineFiles),
                    Keybind = new KeybindsManager.Keybind
                    {
                        KeybindType = item.LinkedKeybind.KeybindType,
                        GlobalKeybindID = item.LinkedKeybind.GlobalKeybindID,
                        Handler = null,
                        Key = item.LinkedKeybind.Key,
                        KeyModifiers = item.LinkedKeybind.KeyModifiers
                    }
                };

                if (entry.Embedded)
                {
                    string destinationFolder = entry.Type == SoundboardFileEntryType.LocalFile ? "LocalSounds" : "OnlineSounds";
                    string destinationPath = Path.Combine(tempFolder, destinationFolder, entry.Name + Path.GetExtension(entry.Path));

                    File.Copy(entry.Path, destinationPath);
                    entry.Path = "\\" + destinationFolder + "\\" + entry.Name + Path.GetExtension(entry.Path);
                }

                SoundboardFileEntries.Add(entry);
            }

            SoundboardFile soundboardFile = new SoundboardFile
            {
                ClankFileVersion = ClankFileVersion,
                Name = name,
                Sounds = SoundboardFileEntries
            };

            File.WriteAllText(JSONPath, JsonConvert.SerializeObject(soundboardFile, Formatting.Indented));

            string zipPath = Path.Combine(Path.GetTempPath(), "Clankboard", "TempSoundboardFiles", name + ".clankboard");
            ZipFile.CreateFromDirectory(tempFolder, zipPath);

            string pathFolder = Path.GetDirectoryName(path);
            File.Move(zipPath, Path.Combine(pathFolder, name + ".clankboard"), true);
        }



        SoundboardFileManager()
        {
        }

        #region Singleton
        public static SoundboardFileManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new();
                    }
                    return instance;
                }
            }
        }
        #endregion
    }

    public class SettingsFileManager
    {
        #region Singleton vars
        private static SettingsFileManager instance = null;
        private static readonly object padlock = new object();
        #endregion

        /// <summary>
        /// This is the settings file that is used by default.
        /// </summary>
        private readonly ClankFile DefaultSettingsFile = new ClankFile { Name = "settings.json", FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clankboard") };

        /// <summary>
        /// The application settings file. Usually located in %APPDATA%\Clankboard\settings.json
        /// </summary>
        public ClankFile SettingsFile { get; private set; }

        public void LoadFile(string path=null)
        {
            if (path == null) 
            {
                SettingsFile = DefaultSettingsFile;

                // if file does not exist, create it.
                if (!File.Exists(
                    Path.Combine(SettingsFile.FolderPath, SettingsFile.Name)))
                {
                    Directory.CreateDirectory(SettingsFile.FolderPath);
                    File.Create(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name)).Close();

                    // Save the default settings to the file
                    File.WriteAllText(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name), JsonConvert.SerializeObject(SettingsManager.GetSettings(), Formatting.Indented));
                    return;
                }

                // Doing this using Newtonsoft.Json
                string json = File.ReadAllText(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name));

                // Deserialize the json into the settings object.
                List<KeyValuePair<SettingsManager.SettingTypes, object>> DeserializedSettings = JsonConvert.DeserializeObject<List<KeyValuePair<SettingsManager.SettingTypes, object>>>(json);
                List<KeyValuePair<SettingsManager.SettingTypes, object>> NewSettings = new List<KeyValuePair<SettingsManager.SettingTypes, object>>();
                AudioManager.AudioDevice tempAudioDevice = new AudioManager.AudioDevice();

                // Convert the settings of type AudioDevice to the correct type. Move all settings to NewSettings, modified or not.
                foreach (KeyValuePair<SettingsManager.SettingTypes, object> setting in DeserializedSettings)
                {
                    switch (setting.Key)
                    {
                        case SettingsManager.SettingTypes.LocalOutputDevice:
                        case SettingsManager.SettingTypes.VACOutputDevice:
                        case SettingsManager.SettingTypes.InputDevice:
                            tempAudioDevice = JsonConvert.DeserializeObject<AudioManager.AudioDevice>(setting.Value.ToString());
                            NewSettings.Add(new KeyValuePair<SettingsManager.SettingTypes, object>(setting.Key, tempAudioDevice));
                            break;
                        default:
                            // convert the setting to the correct type. If the value is null, just add it as null.
                            //NewSettings.Add(new KeyValuePair<SettingsManager.SettingTypes, object>(setting.Key, Convert.ChangeType(setting.Value, SettingsManager.GetSettingType(setting.Key))));
                            if (setting.Value == null)
                            {
                                NewSettings.Add(new KeyValuePair<SettingsManager.SettingTypes, object>(setting.Key, null));
                            }
                            else
                            {
                                // Check if type is long
                                if (setting.Value.GetType() == typeof(long))
                                {
                                    NewSettings.Add(new KeyValuePair<SettingsManager.SettingTypes, object>(setting.Key, Convert.ToInt32(setting.Value)));
                                }
                                else
                                {
                                    NewSettings.Add(new KeyValuePair<SettingsManager.SettingTypes, object>(setting.Key, setting.Value));
                                }

                                //NewSettings.Add(new KeyValuePair<SettingsManager.SettingTypes, object>(setting.Key, Convert.ChangeType(setting.Value, SettingsManager.GetSettingType(setting.Key))

                            }

                            break;
                    }
                }

                

                // Set the settings
                foreach (KeyValuePair<SettingsManager.SettingTypes, object> setting in NewSettings)
                {
                    SettingsManager.SetSetting(setting.Key, setting.Value);
                }
            }
            else
            {
                throw new NotImplementedException("Custom settings files are not supported yet. Coming soon™");
            }

            // TODO: Add loading code
        }

        public void SaveFile(string path=null)
        {
            if (path == null)
            {
                SettingsFile = DefaultSettingsFile;

                // if file does not exist, create it.
                if (!File.Exists(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name)))
                {
                    Directory.CreateDirectory(SettingsFile.FolderPath);
                    File.Create(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name)).Close();
                }

                // Save the settings to the file
                File.WriteAllText(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name), JsonConvert.SerializeObject(SettingsManager.GetSettings(), Formatting.Indented));
            }
            else
            {
                throw new NotImplementedException("Custom settings files are not supported yet. Coming soon™");
            }
        }



        static SettingsFileManager()
        {
        }

        private SettingsFileManager()
        {
        }

        #region Singleton
        public static SettingsFileManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new();
                    }
                    return instance;
                }
            }
        }
        #endregion
    }
}
