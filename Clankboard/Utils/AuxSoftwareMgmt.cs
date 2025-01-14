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

        public static string AuxSoftwareFolder { get; private set; }
        public static string YTDLPPath { get; private set; }
        public static string FFMpegPath { get; private set; }

        public static async Task<bool> CheckYTDLP()
        {
            CheckAuxSoftwareFolder();

            YTDLPPath = System.IO.Path.Combine(AuxSoftwareFolder, "yt-dlp.exe");

            if (!File.Exists(YTDLPPath))
            {
                // Download ytdlp.exe
                //await YoutubeDLSharp.Utils.DownloadYtDlp(AuxSoftwareFolder);
            }
            else
            {
                // Run yt-dlp.exe -U (YT-DLP Path)
                // Library does not support this yet, so just run the exe with the -U flag
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = YTDLPPath,
                    Arguments = "-U -loglevel 0",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process process = Process.Start(startInfo);
                process.WaitForExit(); // Wait for the process to finish
            }

            return true;
        }

        public static async Task<bool> CheckFFMpeg() 
        {
            CheckAuxSoftwareFolder();

            FFMpegPath = System.IO.Path.Combine(AuxSoftwareFolder, "ffmpeg.exe");

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

    public enum DownloadResult
    {
        Success,                // The download has succeeded. The file is now in the file system.
        NoInternet,             // The OS reports no internet connectivity. Happens when not being connected to any network.
        ServerNotReached,       // The webserver could not be reached.
        InvalidData,            // Data is invalid. Happens when media fails to download for example. (Broken file)
        CertificateInvalid,     // HTTPS Certificate of the webserver is invalid.
    }

    public enum UpdateCheckResult
    {
        UpToDate,               // The software is up to date.
        UpdateAvailable,        // An update is available.
        NotInstalled,           // The software is not installed.
        NoInternet,             // The OS reports no internet connectivity. Happens when not being connected to any network.
        ServerNotReached,       // The webserver could not be reached.
        CertificateInvalid,     // HTTPS Certificate of the webserver is invalid.
    }

    /// <summary>
    /// Represents auxiliary software used by ClankBoard.
    /// </summary>
    public class AuxSoftware 
    {
        public string currentVersion { get; private set; }
        public string filePath { get; private set; }
        private readonly string latestVersionDownloadLink;

        public static List<AuxSoftware> auxSoftwares { get; private set; }

        public AuxSoftware(string filePath, string latestVersionDownloadLink)
        {
            this.filePath = filePath;
            this.latestVersionDownloadLink = latestVersionDownloadLink;
        }

        public UpdateCheckResult CheckForUpdates()
        {
            if (!File.Exists(filePath)) 
                return UpdateCheckResult.NotInstalled;



            return UpdateCheckResult.UpToDate;
        }

        public DownloadResult DownloadLatestVersion() 
        {
            return DownloadResult.Success;
        } 
    }
}
