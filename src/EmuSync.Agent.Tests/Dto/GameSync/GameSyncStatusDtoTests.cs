using EmuSync.Agent.Dto.GameSync;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.GameSync;

public class GameSyncStatusDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new GameSyncStatusDto
        {
            LastSyncedFrom = "s",
            RequiresDownload = true,
            RequiresUpload = false,
            LocalFolderPathExists = true
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"lastSyncedFrom\"", json);
        Assert.Contains("\"requiresDownload\"", json);
        Assert.Contains("\"requiresUpload\"", json);
        Assert.Contains("\"localFolderPathExists\"", json);
    }
}
