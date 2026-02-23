using EmuSync.Services.Storage.Dropbox;

namespace EmuSync.Services.Storage.Tests.Dropbox;

public class DropboxStorageProviderConfigTests
{
    [Fact]
    public void Section_Is_Correct()
    {
        Assert.Equal("DropboxStorageProviderConfig", DropboxStorageProviderConfig.Section);
    }
}
