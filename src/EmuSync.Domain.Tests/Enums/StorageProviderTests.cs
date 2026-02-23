using EmuSync.Domain.Enums;

namespace EmuSync.Domain.Tests.Enums;

public class StorageProviderTests
{
    [Fact]
    public void HasExpectedCount()
    {
        var values = Enum.GetValues<StorageProvider>();
        Assert.Equal(4, values.Length);
    }

    [Theory]
    [InlineData(1, StorageProvider.GoogleDrive)]
    [InlineData(2, StorageProvider.Dropbox)]
    [InlineData(3, StorageProvider.OneDrive)]
    [InlineData(4, StorageProvider.SharedFolder)]
    public void HasExpectedValueAndUnderlyingInt(int underlying, StorageProvider provider)
    {
        Assert.Equal(underlying, (int)provider);
    }
}