using EmuSync.Agent.Services;
using EmuSync.Domain.Enums;

namespace EmuSync.Agent.Tests.Services;

public class GameSyncStatusCacheTests
{
    [Fact]
    public void AddOrUpdate_And_Get_Work()
    {
        var cache = new GameSyncStatusCache();

        cache.AddOrUpdate("g1", GameSyncStatus.RequiresDownload);

        var val = cache.Get("g1");

        Assert.Equal(GameSyncStatus.RequiresDownload, val);
    }
}
