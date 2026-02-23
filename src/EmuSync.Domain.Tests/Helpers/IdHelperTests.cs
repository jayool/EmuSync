using EmuSync.Domain.Helpers;

namespace EmuSync.Domain.Tests.Helpers;

public class IdHelperTests
{
    [Fact]
    public void Create_Returns_NonEmpty_Guid_N()
    {
        var id = IdHelper.Create();
        Assert.False(string.IsNullOrEmpty(id));
        Assert.Equal(32, id.Length);
    }
}
