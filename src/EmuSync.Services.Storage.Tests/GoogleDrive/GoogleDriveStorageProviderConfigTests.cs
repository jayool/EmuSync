using EmuSync.Services.Storage.GoogleDrive;

namespace EmuSync.Services.Storage.Tests.GoogleDrive;

public class GoogleDriveStorageProviderConfigTests
{
    [Fact]
    public void Section_Is_Correct()
    {
        Assert.Equal("GoogleDriveStorageProviderConfig", GoogleDriveStorageProviderConfig.Section);
    }
}
