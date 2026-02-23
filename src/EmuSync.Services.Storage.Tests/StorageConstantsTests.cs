namespace EmuSync.Services.Storage.Tests;

public class StorageConstantsTests
{
    [Fact]
    public void Constants_Are_Correct()
    {
        Assert.Equal("EmuSync", StorageConstants.ApplicationName);
        Assert.Equal("EmuSync_Data", StorageConstants.DataFolderName);
        Assert.Equal("games", StorageConstants.GamesFolderName);
        Assert.Equal("game-{0}-metadata.json", StorageConstants.FileName_GameMetaData);
        Assert.Equal("game-{0}.zip", StorageConstants.FileName_GameZip);
        Assert.Equal("game-list.json", StorageConstants.FileName_GameList);
        Assert.Equal("sync-sources.json", StorageConstants.FileName_SyncSourceList);
    }
}
