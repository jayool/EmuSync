using EmuSync.Agent.Dto.Game;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Game;

public class GameSummaryDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new GameSummaryDto
        {
            Id = "id",
            Name = "n",
            AutoSync = true,
            MaximumLocalGameBackups = 2,
            SyncSourceIdLocations = new Dictionary<string, string> { { "a", "b" } },
            LastSyncedFrom = "x",
            LastSyncTimeUtc = DateTime.UtcNow,
            SyncStatusId = 1,
            StorageBytes = 10
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"id\"", json);
        Assert.Contains("\"name\"", json);
        Assert.Contains("\"autoSync\"", json);
        Assert.Contains("\"maximumLocalGameBackups\"", json);
        Assert.Contains("\"syncSourceIdLocations\"", json);
        Assert.Contains("\"lastSyncedFrom\"", json);
        Assert.Contains("\"lastSyncTimeUtc\"", json);
        Assert.Contains("\"syncStatusId\"", json);
        Assert.Contains("\"storageBytes\"", json);
    }
}
