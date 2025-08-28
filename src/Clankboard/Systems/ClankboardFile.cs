using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Clankboard.AudioSystem;

namespace Clankboard.Systems;

public struct ClankboardFileEntry
{
    private SoundboardItemType type;
    private string name;
    private string path;
    private string physicalPath;
    private bool directThroughVoicebox;
    private bool dataEmbedded;

    public ClankboardFileEntry(SoundboardItemType type, string name, string path, string physicalPath,
        bool directThroughVoicebox)
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
///     Class which controls software file saving/loading behaviour.
/// </summary>
public class ClankboardFile : LoadedClankFile
{
    private readonly string path;

    public List<ClankboardFileEntry> clankboardFileEntries = new();
    public string clankboardFileVersion;

    public ClankboardFile(string path) : base(path)
    {
        this.path = path;
    }

    public async void Load()
    {
        // Check if the file even exists
        if (!File.Exists(path)) throw new FileNotFoundException("File not found at path: " + path);

        // Unzip to temp folder "clankTemp[4 random numbers]_[file name]"
        var tempFolder = Path.Combine(Path.GetTempPath(),
            "clankTemp" + new Random().Next(1000, 9999) + "_" + Path.GetFileName(path));
        Directory.CreateDirectory(tempFolder);

        // Unzip the file
        ZipFile.ExtractToDirectory(path, tempFolder);

        // Load the file
        var json = File.ReadAllText(Path.Combine(tempFolder, "clankboardFile.json"));
        // Serialize the JSON file to a List<>
    }
}