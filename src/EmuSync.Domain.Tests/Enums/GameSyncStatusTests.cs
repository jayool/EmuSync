using EmuSync.Domain.Enums;

namespace EmuSync.Domain.Tests.Enums;

public class GameSyncStatusTests
{
    [Fact]
    public void HasExpectedCount()
    {
        var values = Enum.GetValues<GameSyncStatus>();
        Assert.Equal(5, values.Length);
    }

    [Theory]
    [InlineData(0, GameSyncStatus.Unknown)]
    [InlineData(1, GameSyncStatus.RequiresDownload)]
    [InlineData(2, GameSyncStatus.RequiresUpload)]
    [InlineData(3, GameSyncStatus.InSync)]
    [InlineData(4, GameSyncStatus.UnsetDirectory)]
    public void HasExpectedValueAndUnderlyingInt(int underlying, GameSyncStatus status)
    {
        Assert.Equal(underlying, (int)status);
    }
}