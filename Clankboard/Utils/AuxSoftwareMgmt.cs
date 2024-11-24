using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Clankboard.Utils
{
    static class AuxSoftwareMgmt
    {

        private static string AuxSoftwareFolder;

        public static async Task<bool> CheckYTDLP()
        {
            CheckAuxSoftwareFolder();

            string YTDLPPath = System.IO.Path.Combine(AuxSoftwareFolder, "ytdlp.exe");

            if (!File.Exists(YTDLPPath))
            {
                // Download ytdlp.exe
                //await YoutubeDLSharp.Utils.DownloadYtDlp(AuxSoftwareFolder);
            }
            else 
            {
                // Run yt-dlp.exe -U (YT-DLP Path)
                // Library does not support this yet, so just run the exe with the -U flag
                Process process = System.Diagnostics.Process.Start(YTDLPPath, "-U");
                process.WaitForExit(); // Wait for the process to finish
            }

            return true;
        }

        public static async Task<bool> CheckFFMpeg() 
        {
            CheckAuxSoftwareFolder();

            string FFMpegPath = System.IO.Path.Combine(AuxSoftwareFolder, "ffmpeg.exe");

            if (!File.Exists(FFMpegPath))
            {
                // Download ffmpeg.exe
                await YoutubeDLSharp.Utils.DownloadFFmpeg(AuxSoftwareFolder);
            }

            return true;
        }

        private static void CheckAuxSoftwareFolder() 
        {
            // Check if AppData Folder\AuxSoftware exists
            AuxSoftwareFolder = System.IO.Path.Combine(App.AppDataPath, "AuxSoftware");

            if (!Directory.Exists(AuxSoftwareFolder))
            {
                // Create the directory
                Directory.CreateDirectory(AuxSoftwareFolder);
            }
        }
    }
}
