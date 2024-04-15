using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

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
        #region Singleton vars
        private static SoundboardFileManager instance = null;
        private static readonly object padlock = new object();
        #endregion

        /// <summary>
        /// Current soundboard file. This is the file that is currently loaded / used.
        /// </summary>
        public ClankFile CurrentSoundboardFile { get; private set; }

        public void LoadFile(string path)
        {
            // Load the soundboard file
        }

        public void SaveFile(string path)
        {
            // Save the soundboard file (USING CURRENT SOUNDBOARD PAGE CONTENTS)
        }



        SoundboardFileManager()
        {
        }

        #region Singleton
        public static SoundboardFileManager GetInstance
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

                // Set the settings
                foreach (KeyValuePair<SettingsManager.SettingTypes, object> setting in DeserializedSettings)
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
