using EmuSync.Agent.Dto.SyncSource;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.SyncSource;

public class SyncSourceSummaryDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new SyncSourceSummaryDto
        {
            Id = "i",
            Name = "n",
            StorageProviderId = 1,
            PlatformId = 2
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"id\"", json);
        Assert.Contains("\"name\"", json);
        Assert.Contains("\"storageProviderId\"", json);
        Assert.Contains("\"platformId\"", json);
    }
}
