namespace EmuSync.Domain.Tests;

public class DomainConstantsTests
{
    [Fact]
    public void DomainConstants_Have_Expected_Values()
    {
        Assert.Equal(".emusync-data", DomainConstants.LocalDataFolder);
        Assert.Equal("game-backups", DomainConstants.LocalDataGameBackupFolder);
        Assert.Equal("temp-zips", DomainConstants.LocalDataGameTempZipsFolder);
        Assert.Equal("backup_{0}.zip", DomainConstants.LocalDataGameBackupFileNameFormat);
        Assert.Equal("manifest.json", DomainConstants.LocalDataGameBackupManifestFile);
        Assert.Equal("sync-source.json", DomainConstants.LocalDataSyncSourceFile);
        Assert.Equal("local-sync-log.log", DomainConstants.LocalDataSyncLogFile);
        Assert.Equal("ludusavi-last-etag.json", DomainConstants.LocalDataLudusaviLastEtagFile);
        Assert.Equal("ludusavi-manifest.json", DomainConstants.LocalDataLudusaviManifestFile);
        Assert.Equal("ludusavi-cached-scan.json", DomainConstants.LocalDataLudusaviCachedScanFile);
        Assert.Equal(10, DomainConstants.DefaultMaximumLocalGameBackups);
    }
}
