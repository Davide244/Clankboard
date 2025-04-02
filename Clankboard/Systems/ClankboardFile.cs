using Clankboard.AudioSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Clankboard.Systems
{
    public struct ClankboardFileEntry
    {
        SoundboardItemType type;
        string name;
        string path;
        string physicalPath;
        bool directThroughVoicebox;
        bool dataEmbedded;

        public ClankboardFileEntry(SoundboardItemType type, string name, string path, string physicalPath, bool directThroughVoicebox)
        {
            this.type = type;
            this.name = name;
            this.path = path;
            this.physicalPath = physicalPath;
            this.directThroughVoicebox = directThroughVoicebox;
        }
    }

    public struct ClankboardFileData 
    {

    }

    /// <summary>
    /// Class which controls software file saving/loading behaviour.
    /// </summary>
    public class ClankboardFile : LoadedClankFile
    {
        public string clankboardFileVersion;
        private string path;

        public List<ClankboardFileEntry> clankboardFileEntries = new List<ClankboardFileEntry>();

        public ClankboardFile(string path) : base(path)
        {
            this.path = path;
        }

        public async void Load()
        {
            // Check if the file even exists
            if (!System.IO.File.Exists(path))
            {
                throw new System.IO.FileNotFoundException("File not found at path: " + path);
            }

            // Unzip to temp folder "clankTemp[4 random numbers]_[file name]"
            string tempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "clankTemp" + new Random().Next(1000, 9999) + "_" + System.IO.Path.GetFileName(path));
            System.IO.Directory.CreateDirectory(tempFolder);

            // Unzip the file
            System.IO.Compression.ZipFile.ExtractToDirectory(path, tempFolder);

            // Load the file
            string json = System.IO.File.ReadAllText(System.IO.Path.Combine(tempFolder, "clankboardFile.json"));
            // Serialize the JSON file to a List<>
        }


    }
}
