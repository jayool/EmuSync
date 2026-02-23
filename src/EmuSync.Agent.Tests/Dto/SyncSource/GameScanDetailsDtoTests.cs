using EmuSync.Agent.Dto.SyncSource;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.SyncSource;

public class GameScanDetailsDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new GameScanDetailsDto
        {
            LastScanSeconds = 10,
            InProgress = true,
            ProgressPercent = 50,
            CountOfGames = 3
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"lastScanSeconds\"", json);
        Assert.Contains("\"inProgress\"", json);
        Assert.Contains("\"progressPercent\"", json);
        Assert.Contains("\"countOfGames\"", json);
    }
}
