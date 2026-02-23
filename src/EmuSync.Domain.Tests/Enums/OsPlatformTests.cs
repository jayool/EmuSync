using EmuSync.Domain.Enums;

namespace EmuSync.Domain.Tests.Enums;

public class OsPlatformTests
{
    [Fact]
    public void HasExpectedCount()
    {
        var values = Enum.GetValues<OsPlatform>();
        Assert.Equal(4, values.Length);
    }

    [Theory]
    [InlineData(0, OsPlatform.Unknown)]
    [InlineData(1, OsPlatform.Windows)]
    [InlineData(2, OsPlatform.Linux)]
    [InlineData(3, OsPlatform.Mac)]
    public void HasExpectedValueAndUnderlyingInt(int underlying, OsPlatform platform)
    {
        Assert.Equal(underlying, (int)platform);
    }
}