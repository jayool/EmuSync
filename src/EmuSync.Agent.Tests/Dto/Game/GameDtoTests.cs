using EmuSync.Agent.Dto.Game;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Game;

public class GameDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new GameDto
        {
            Id = "i",
            Name = "n",
            AutoSync = true,
            MaximumLocalGameBackups = 1,
            SyncSourceIdLocations = new Dictionary<string, string> { { "a", "b" } },
            LastSyncedFrom = "x",
            LastSyncTimeUtc = DateTime.UtcNow,
            StorageBytes = 100
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"id\"", json);
        Assert.Contains("\"name\"", json);
        Assert.Contains("\"autoSync\"", json);
        Assert.Contains("\"maximumLocalGameBackups\"", json);
        Assert.Contains("\"syncSourceIdLocations\"", json);
        Assert.Contains("\"lastSyncedFrom\"", json);
        Assert.Contains("\"lastSyncTimeUtc\"", json);
        Assert.Contains("\"storageBytes\"", json);
    }
}
