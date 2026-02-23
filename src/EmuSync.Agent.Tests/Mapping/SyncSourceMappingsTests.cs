using EmuSync.Agent.Dto.SyncSource;
using EmuSync.Agent.Mapping;
using EmuSync.Domain.Entities;
using EmuSync.Domain.Enums;

namespace EmuSync.Agent.Tests.Mapping;

public class SyncSourceMappingsTests
{
    [Fact]
    public void SyncSourceEntity_MapsTo_SyncSourceDto()
    {
        var entity = new SyncSourceEntity
        {
            Id = "id",
            Name = "n",
            StorageProvider = StorageProvider.OneDrive,
            OsPlatform = OsPlatform.Windows,
            AutoSyncFrequency = TimeSpan.FromMinutes(30),
            MaximumLocalGameBackups = 7
        };

        SyncSourceDto dto = entity.ToDto();

        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal((int?)entity.StorageProvider, dto.StorageProviderId);
        Assert.Equal((int)entity.OsPlatform, dto.PlatformId);
        Assert.Equal(30, dto.AutoSyncFrequencyMins);
        Assert.Equal(7, dto.MaximumLocalGameBackups);
    }

    [Fact]
    public void SyncSourceEntity_MapsTo_SyncSourceSummaryDto()
    {
        var entity = new SyncSourceEntity
        {
            Id = "id",
            Name = "n",
            StorageProvider = StorageProvider.Dropbox,
            OsPlatform = OsPlatform.Linux
        };

        var dto = entity.ToSummaryDto();

        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal((int?)entity.StorageProvider, dto.StorageProviderId);
        Assert.Equal((int)entity.OsPlatform, dto.PlatformId);
    }

    [Fact]
    public void UpdateSyncSourceDto_MapsTo_SyncSourceEntity()
    {
        var update = new UpdateSyncSourceDto
        {
            Name = "n",
            AutoSyncFrequencyMins = 15,
            MaximumLocalGameBackups = 4
        };

        var entity = update.ToEntity();

        Assert.Equal(update.Name, entity.Name);
        Assert.Equal(TimeSpan.FromMinutes(15), entity.AutoSyncFrequency);
        Assert.Equal(4, entity.MaximumLocalGameBackups);
    }
}
