using EmuSync.Domain.Enums;

namespace EmuSync.Domain.Tests.Enums;

public class SyncTypeTests
{
    [Fact]
    public void HasExpectedCount()
    {
        var values = Enum.GetValues<SyncType>();
        Assert.Equal(2, values.Length);
    }

    [Theory]
    [InlineData(1, SyncType.Upload)]
    [InlineData(2, SyncType.Download)]
    public void HasExpectedValueAndUnderlyingInt(int underlying, SyncType syncType)
    {
        Assert.Equal(underlying, (int)syncType);
    }
}