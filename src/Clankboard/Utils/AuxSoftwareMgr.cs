using System;
using System.IO;

namespace Clankboard.Utils;

public sealed class AppDataFolderManager
{
    private readonly string clankAppDataFolder;

    public AppDataFolderManager()
    {
        clankAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Clankboard");

        if (!Directory.Exists(clankAppDataFolder)) Directory.CreateDirectory(clankAppDataFolder);

        auxSoftwareHelper = new AuxSoftwareHelper(clankAppDataFolder);
    }

    public AuxSoftwareHelper auxSoftwareHelper { get; private set; }

    public string GetAppDataFolder(bool checkAppdataFolders = true)
    {
        if (checkAppdataFolders) CheckAndCreateAppDataFolders();
        return clankAppDataFolder;
    }

    private void CheckAndCreateAppDataFolders()
    {
        if (!Directory.Exists(clankAppDataFolder)) Directory.CreateDirectory(clankAppDataFolder);
    }
}

public sealed class AuxSoftwareHelper
{
    private string ffmpegPath;
    private string ffprobePath;
    private string ytDlpPath;

    public AuxSoftwareHelper(string appDataFolder)
    {
        auxSoftwareFolder = Path.Combine(appDataFolder, "AuxSoftware");
        if (!Directory.Exists(auxSoftwareFolder)) Directory.CreateDirectory(auxSoftwareFolder);
    }

    public string auxSoftwareFolder { get; }

    public string GetYtDlpPath()
    {
        return ytDlpPath;
    }

    public string GetFfmpegPath()
    {
        return ffmpegPath;
    }

    public string GetFfprobePath()
    {
        return ffprobePath;
    }

    /// <summary>
    ///     Check if yt-dlp is installed
    /// </summary>
    /// <returns>bool which indicates if yt-dlp is installed.</returns>
    public bool checkYtDlpPath()
    {
        ytDlpPath = Path.Combine(auxSoftwareFolder, "yt-dlp.exe");
        return File.Exists(ytDlpPath);
    }

    /// <summary>
    ///     Check if ffmpeg is installed
    /// </summary>
    /// <returns>bool which indicates if ffmpeg is installed.</returns>
    public bool checkFfmpegPath()
    {
        ffmpegPath = Path.Combine(auxSoftwareFolder, "ffmpeg.exe");
        return File.Exists(ffmpegPath);
    }

    /// <summary>
    ///     Check if ffprobe is installed
    /// </summary>
    /// <returns>bool which indicates if ffprobe is installed.</returns>
    public bool checkFfprobePath()
    {
        ffprobePath = Path.Combine(auxSoftwareFolder, "ffprobe.exe");
        return File.Exists(ffprobePath);
    }
}