using EmuSync.Agent.Dto.SyncSource;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.SyncSource;

public class NextAutoSyncTimeDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new NextAutoSyncTimeDto
        {
            SecondsLeft = 123.4
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"secondsLeft\"", json);
    }
}
