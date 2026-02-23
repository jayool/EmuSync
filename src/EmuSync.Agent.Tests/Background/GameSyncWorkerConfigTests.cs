using EmuSync.Agent.Background;

namespace EmuSync.Agent.Tests.Background;

public class GameSyncWorkerConfigTests
{
    [Fact]
    public void Section_HasExpectedValue()
    {
        Assert.Equal("GameSyncWorkerConfig", GameSyncWorkerConfig.Section);
    }
}