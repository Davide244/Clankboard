﻿using Newtonsoft.Json.Bson;
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

        public string GetAppDataFolder(bool checkAppdataFolders = true) 
        {
            if (checkAppdataFolders) CheckAndCreateAppDataFolders();
            return clankAppDataFolder;
        }

        public AppDataFolderManager() 
        {
            clankAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clankboard");

            if (!Directory.Exists(clankAppDataFolder))
            {
                Directory.CreateDirectory(clankAppDataFolder);
            }

            auxSoftwareHelper = new AuxSoftwareHelper(clankAppDataFolder);


        }

        private void CheckAndCreateAppDataFolders()
        {
            if (!Directory.Exists(clankAppDataFolder))
            {
                Directory.CreateDirectory(clankAppDataFolder);
            }
        }
    }

    public sealed class AuxSoftwareHelper 
    {
        public string auxSoftwareFolder { get; private set; }
        private string ytDlpPath;
        private string ffmpegPath;
        private string ffprobePath;

        public string GetYtDlpPath() => ytDlpPath;
        public string GetFfmpegPath() => ffmpegPath;
        public string GetFfprobePath() => ffprobePath;

        public AuxSoftwareHelper(string appDataFolder) 
        {
            auxSoftwareFolder = Path.Combine(appDataFolder, "AuxSoftware");
            if (!Directory.Exists(auxSoftwareFolder))
            {
                Directory.CreateDirectory(auxSoftwareFolder);
            }
        }

        /// <summary>
        /// Check if yt-dlp is installed
        /// </summary>
        /// <returns>bool which indicates if yt-dlp is installed.</returns>
        public bool checkYtDlpPath()
        {
            ytDlpPath = Path.Combine(auxSoftwareFolder, "yt-dlp.exe");
            return File.Exists(ytDlpPath);
        }

        /// <summary>
        /// Check if ffmpeg is installed
        /// </summary>
        /// <returns>bool which indicates if ffmpeg is installed.</returns>
        public bool checkFfmpegPath()
        {
            ffmpegPath = Path.Combine(auxSoftwareFolder, "ffmpeg.exe");
            return File.Exists(ffmpegPath);
        }

        /// <summary>
        /// Check if ffprobe is installed
        /// </summary>
        /// <returns>bool which indicates if ffprobe is installed.</returns>
        public bool checkFfprobePath()
        {
            ffprobePath = Path.Combine(auxSoftwareFolder, "ffprobe.exe");
            return File.Exists(ffprobePath);
        }
    }
}
