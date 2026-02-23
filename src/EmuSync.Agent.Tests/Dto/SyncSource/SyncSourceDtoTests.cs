using EmuSync.Agent.Dto.SyncSource;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.SyncSource;

public class SyncSourceDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new SyncSourceDto
        {
            Id = "i",
            Name = "n",
            StorageProviderId = 1,
            PlatformId = 2,
            AutoSyncFrequencyMins = 15,
            MaximumLocalGameBackups = 5
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"id\"", json);
        Assert.Contains("\"name\"", json);
        Assert.Contains("\"storageProviderId\"", json);
        Assert.Contains("\"platformId\"", json);
        Assert.Contains("\"autoSyncFrequencyMins\"", json);
        Assert.Contains("\"maximumLocalGameBackups\"", json);
    }
}
