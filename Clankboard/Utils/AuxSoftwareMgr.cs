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
    public static class AppDataFolderHelper
    {
        public static string ClankAppDataFolder { get; private set; }
        public static string AuxSoftwareFolder { get; private set; }

        public static readonly string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static void CheckAndCreateAppDataFolders() 
        {
            ClankAppDataFolder = Path.Combine(appDataDir, "Clankboard");
            AuxSoftwareFolder = Path.Combine(ClankAppDataFolder, "AuxSoftware");

            // FOLDER STRUCTURE IN APPDATA\ROAMING:
            // Clankboard
            //    |
            //    |--- AuxSoftware

            try 
            {
                if (!Directory.Exists(ClankAppDataFolder))
                    Directory.CreateDirectory(ClankAppDataFolder);

                if (!Directory.Exists(Path.Combine(ClankAppDataFolder, "AuxSoftware")))
                    Directory.CreateDirectory(Path.Combine(ClankAppDataFolder, "AuxSoftware"));

            }
            catch (Exception e)
            {
                Debug.WriteLine("******** FAILED TO CREATE APP DATA DIR STRUCTURE ********\n*\n* " + e.Message);
            }
        }
    }

    /// <summary>
    /// Manages auxiliary software used by ClankBoard.
    /// Currently, the only auxiliary software used by ClankBoard is yt-dlp and ffmpeg.
    /// </summary>
    public sealed class AuxSoftwareMgr
    {
        public string YTDLPPath { get; private set; }
        public string FFmpegPath { get; private set; }
        public string FFProbePath { get; private set; }

        private const string YTDLPLatestVersionDownloadLink = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
        private const string FFmpegLatestVersionDownloadLink = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.7z";

        public AuxSoftwareMgr() 
        {
            AppDataFolderHelper.CheckAndCreateAppDataFolders();

            YTDLPPath = Path.Combine(AppDataFolderHelper.AuxSoftwareFolder, "yt-dlp.exe");
        }

        public async Task<DownloadResult> UpdateYTDLP() 
        {
            System.Diagnostics.Debug.WriteLine("APPDATA: " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            if (!InetHelper.IsInternetAvailable())
                return DownloadResult.NoInternet;

            if (YTDLPPath == null || !File.Exists(YTDLPPath)) 
            {
                // Download yt-dlp.exe from the official GitHub repository.
                System.Diagnostics.Debug.WriteLine("Re-downloading yt-dlp.exe. \"" + YTDLPPath + "\" does not exist.");

                try 
                {
                    //await YoutubeDLSharp.Utils.DownloadYtDlp(AppDataFolderHelper.AuxSoftwareFolder);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to download yt-dlp.exe: " + e.Message);
                    return DownloadResult.ServerNotReached;
                }

                return DownloadResult.Success;
            }

            // Run yt-dlp.exe -U to check for updates and update YT-DLP. Run with no visible console.
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = YTDLPPath,
                Arguments = "-U",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            return DownloadResult.Success;
        }

        public async Task<DownloadResult> UpdateFFMpeg() 
        {
            if (!InetHelper.IsInternetAvailable())
                return DownloadResult.NoInternet;

            if (!CheckFFMpeg() || FFmpegPath.Contains(@"\Clankboard\AuxSoftware\")) // If ffmpeg is not installed OR if FFMPEG is installed by clankboard, download it.
            {
                // Download ffmpeg from the official website.
                DownloadResult downloadResult = await InetHelper.DownloadFile(FFmpegLatestVersionDownloadLink, Path.Combine(AppDataFolderHelper.AuxSoftwareFolder, "ffmpeg.7z"));
                return downloadResult;
            }

            return DownloadResult.Success;
        }

        private bool CheckFFMpeg()
        {
            // Check if ffmpeg is installed. Both ffmpeg and ffprobe are required. "C:\ffmpeg\bin\ffmpeg.exe" AND "AuxSoftwareFolder\ffmpeg.exe" are acceptable. Same for ffprobe.
            FFmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe";
            FFProbePath = @"C:\ffmpeg\bin\ffprobe.exe";

            if (!File.Exists(FFmpegPath) || !File.Exists(FFProbePath))
            {
                FFmpegPath = Path.Combine(AppDataFolderHelper.AuxSoftwareFolder, "ffmpeg.exe");
                FFProbePath = Path.Combine(AppDataFolderHelper.AuxSoftwareFolder, "ffprobe.exe");
            }

            if (!File.Exists(FFmpegPath) || !File.Exists(FFProbePath))
                return false;

            return true;
        }
    }

    /// <summary>
    /// Represents auxiliary software used by ClankBoard.
    /// </summary>
    //public class AuxSoftware 
    //{
    //    public string currentVersion { get; private set; }
    //    public string filePath { get; private set; }
    //    private readonly string latestVersionDownloadLink;

    //    public static List<AuxSoftware> auxSoftwares { get; private set; }

    //    public AuxSoftware(string filePath, string latestVersionDownloadLink)
    //    {
    //        this.filePath = filePath;
    //        this.latestVersionDownloadLink = latestVersionDownloadLink;
    //    }

    //    public UpdateCheckResult CheckForUpdates()
    //    {
    //        if (!File.Exists(filePath)) 
    //            return UpdateCheckResult.NotInstalled;



    //        return UpdateCheckResult.UpToDate;
    //    }

    //    public DownloadResult DownloadLatestVersion() 
    //    {
    //        return DownloadResult.Success;
    //    } 
    //}
}
