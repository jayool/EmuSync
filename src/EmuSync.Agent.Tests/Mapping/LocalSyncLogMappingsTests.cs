using EmuSync.Agent.Dto.LocalSyncLog;
using EmuSync.Agent.Mapping;
using EmuSync.Domain.Entities;
using EmuSync.Domain.Enums;

namespace EmuSync.Agent.Tests.Mapping;

public class LocalSyncLogMappingsTests
{
    [Fact]
    public void LocalSyncLogEntity_MapsTo_LocalSyncLogDto()
    {
        var entity = new LocalSyncLogEntity
        {
            Id = "id",
            GameId = "g",
            IsAutoSync = true,
            SyncTimeUtc = DateTime.UtcNow,
            SyncType = SyncType.Upload
        };

        LocalSyncLogDto dto = entity.ToDto();

        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.GameId, dto.GameId);
        Assert.Equal(entity.IsAutoSync, dto.IsAutoSync);
        Assert.Equal(entity.SyncTimeUtc, dto.SyncTimeUtc);
        Assert.Equal(entity.SyncType, dto.SyncType);
    }
}
