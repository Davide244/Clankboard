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
    /// <summary>
    /// Manages auxiliary software used by ClankBoard.
    /// Currently, the only auxiliary software used by ClankBoard is yt-dlp and ffmpeg.
    /// </summary>
    public sealed class AuxSoftwareMgr
    {
        private readonly string AuxSoftwareFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Clankboard\AuxSoftware\");

        public string YTDLPPath { get; private set; }
        public string FFmpegPath { get; private set; }
        public string FFProbePath { get; private set; }

        private const string YTDLPLatestVersionDownloadLink = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
        private const string FFmpegLatestVersionDownloadLink = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.7z";

        public AuxSoftwareMgr() 
        {
            if (!Directory.Exists(AuxSoftwareFolder)) 
                Directory.CreateDirectory(AuxSoftwareFolder);

            YTDLPPath = Path.Combine(AuxSoftwareFolder, "yt-dlp.exe");
        }

        public async Task<DownloadResult> UpdateYTDLP() 
        {
            if (!InetHelper.IsInternetAvailable())
                return DownloadResult.NoInternet;

            if (YTDLPPath == null || !File.Exists(YTDLPPath)) 
            {
                // Download yt-dlp.exe from the official GitHub repository.
                System.Diagnostics.Debug.WriteLine("Re-downloading yt-dlp.exe");
                try 
                {
                    await YoutubeDLSharp.Utils.DownloadYtDlp(AuxSoftwareFolder);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    return DownloadResult.ServerNotReached;
                }

                return DownloadResult.Success;
            }

            // Run yt-dlp.exe -U to check for updates and update YT-DLP. Run with no visible console.
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = YTDLPPath,
                Arguments = "-U",
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
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
                DownloadResult downloadResult = await InetHelper.DownloadFile(FFmpegLatestVersionDownloadLink, Path.Combine(AuxSoftwareFolder, "ffmpeg.7z"));
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
                FFmpegPath = Path.Combine(AuxSoftwareFolder, "ffmpeg.exe");
                FFProbePath = Path.Combine(AuxSoftwareFolder, "ffprobe.exe");
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
