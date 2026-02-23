using EmuSync.Domain.Enums;
using EmuSync.Domain.Helpers;

namespace EmuSync.Domain.Tests.Helpers;

public class PlatformHelperTests
{
    [Fact]
    public void GetOsPlatform_Returns_ValidEnum()
    {
        var result = PlatformHelper.GetOsPlatform();
        Assert.IsType<OsPlatform>(result);
        Assert.True(Enum.IsDefined(typeof(OsPlatform), result));
    }
}
