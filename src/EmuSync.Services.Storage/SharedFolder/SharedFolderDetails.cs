namespace EmuSync.Services.Storage.SharedFolder;

public record SharedFolderDetails
{
    /// <summary>
    /// The path to the shared folder
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Optional username for accessing the folder
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Optional password for accessing the folder
    /// </summary>
    public string? Password { get; set; }

    public bool IsWindowsShared()
    {
        return !string.IsNullOrEmpty(Path)
            && Path.StartsWith(@"\\")
            && !string.IsNullOrEmpty(Username)
            && !string.IsNullOrEmpty(Password);
    }
}