using EmuSync.Agent.Dto.Auth;
using EmuSync.Services.Storage.SharedFolder;

namespace EmuSync.Agent.Mapping;

public static class AuthMapping
{
    /// <summary>
    /// Maps a <see cref="SharedFolderAuthFinishDto"/> to a <see cref="SharedFolderDetails"/>
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static SharedFolderDetails ToSharedFolderDetails(this SharedFolderAuthFinishDto dto)
    {
        return new()
        {
            Path = dto.Path,
            Username = dto.Username,
            Password = dto.Password
        };
    }
}
