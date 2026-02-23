using EmuSync.Services.Storage.OneDrive;

namespace EmuSync.Services.Storage.Tests.OneDrive;

public class OneDriveStorageProviderConfigTests
{
    [Fact]
    public void Section_Is_Correct()
    {
        Assert.Equal("OneDriveStorageProviderConfig", OneDriveStorageProviderConfig.Section);
    }
}
