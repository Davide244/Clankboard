using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                    File.WriteAllText(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name), JsonConvert.SerializeObject(SettingsManager.GetSettings()));
                    return;
                }

                // Deserialize the json into the settings object.
                // JSON STRUCTURE:
                // {
                //     "Settings": [
                //         {
                //             "type": 1,
                //             "name": "LocalOutputVolume",
                //             "value": 100
                //         }
                //     ]
                // }

                // Doing this using Newtonsoft.Json
                string json = File.ReadAllText(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name));

                // Open the file in notepad.exe
                Process.Start("notepad.exe", Path.Combine(SettingsFile.FolderPath, SettingsFile.Name));

                // open handle to file
                using (FileStream fs = File.Open(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // Lock the file
                    fs.Lock(0, fs.Length);
                    // Write to the file
                    fs.Write(Encoding.UTF8.GetBytes("Hello World!"), 0, Encoding.UTF8.GetBytes("Hello World!").Length);
                }

                // Deserialize the json into the settings object.
                List<KeyValuePair<SettingsManager.SettingTypes, object>> DeserializedSettings = JsonConvert.DeserializeObject<List<KeyValuePair<SettingsManager.SettingTypes, object>>>(json);
            }
            else
            {
                throw new NotImplementedException("Loading settings from a custom path is not supported yet. Coming soon™");
            }

            // TODO: Add loading code
        }

        public void SaveFile(string path)
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

                // Deserialize the json into the settings object.
                // JSON STRUCTURE:
                // {
                //     "Settings": [
                //         {
                //             "type": 1,
                //             "name": "LocalOutputVolume",
                //             "value": 100
                //         }
                //     ]
                // }

                // Doing this using Newtonsoft.Json
                string json = File.ReadAllText(Path.Combine(SettingsFile.FolderPath, SettingsFile.Name));

                // Deserialize the json into the settings object.
                List<KeyValuePair<SettingsManager.SettingTypes, object>> DeserializedSettings = JsonConvert.DeserializeObject<List<KeyValuePair<SettingsManager.SettingTypes, object>>>(json);
            }
            else
            {
                throw new NotImplementedException("Loading settings from a custom path is not supported yet. Coming soon™");
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
