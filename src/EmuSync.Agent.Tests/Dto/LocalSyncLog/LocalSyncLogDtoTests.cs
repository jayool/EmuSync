using EmuSync.Agent.Dto.LocalSyncLog;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.LocalSyncLog;

public class LocalSyncLogDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new LocalSyncLogDto
        {
            Id = "i",
            GameId = "g",
            IsAutoSync = true,
            SyncTimeUtc = DateTime.UtcNow,
            SyncType = EmuSync.Domain.Enums.SyncType.Download
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"id\"", json);
        Assert.Contains("\"gameId\"", json);
        Assert.Contains("\"isAutoSync\"", json);
        Assert.Contains("\"syncTimeUtc\"", json);
        Assert.Contains("\"syncType\"", json);
    }
}
