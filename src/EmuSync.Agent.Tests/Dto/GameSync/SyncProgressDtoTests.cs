using EmuSync.Agent.Dto.GameSync;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.GameSync;

public class SyncProgressDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new SyncProgressDto
        {
            InProgress = true,
            OverallCompletionPercent = 50,
            CurrentStage = "s"
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"inProgress\"", json);
        Assert.Contains("\"overallCompletionPercent\"", json);
        Assert.Contains("\"currentStage\"", json);
    }
}
