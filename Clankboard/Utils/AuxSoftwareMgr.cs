using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Utils
{
    public sealed class AppDataFolderManager 
    {
        public AuxSoftwareHelper auxSoftwareHelper { get; private set; }
        private string clankAppDataFolder;

        public string GetAppDataFolder() 
        {
            CheckAndCreateAppDataFolders();
            return clankAppDataFolder;
        }

        public AppDataFolderManager() 
        {
            clankAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clankboard");
        }

        public void CheckAndCreateAppDataFolders()
        {
            if (!Directory.Exists(clankAppDataFolder))
            {
                Directory.CreateDirectory(clankAppDataFolder);
            }
        }
    }

    public sealed class AuxSoftwareHelper 
    {
        private string ytDlpPath;
        private string ffmpegPath;
        private string ffprobePath;

        public string GetYtDlpPath() => ytDlpPath;
        public string GetFfmpegPath() => ffmpegPath;
        public string GetFfprobePath() => ffprobePath;

        public AuxSoftwareHelper(string appDataFolder) 
        {

        }
    }
}
