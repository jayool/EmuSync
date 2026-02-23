using EmuSync.Agent.Services;
using EmuSync.Agent.Services.Interfaces;
using EmuSync.Domain.Entities;
using EmuSync.Domain.Enums;
using EmuSync.Services.Managers.Interfaces;
using EmuSync.Services.Managers.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmuSync.Agent.Tests.Services;

public class GameSyncServiceTests
{
    private static (
        GameSyncService Sut,
        Mock<ISyncTasks> SyncTasks,
        Mock<IGameSyncStatusCache> Cache,
        Mock<IGameManager> GameManager,
        Mock<IGameSyncManager> GameSyncManager,
        Mock<ISyncSourceManager> SyncSourceManager
    ) CreateSut()
    {
        var logger = new Mock<ILogger<GameSyncService>>();
        var syncTasks = new Mock<ISyncTasks>();
        var cache = new Mock<IGameSyncStatusCache>();
        var gameManager = new Mock<IGameManager>();
        var gameSyncManager = new Mock<IGameSyncManager>();
        var syncSourceManager = new Mock<ISyncSourceManager>();

        var sut = new GameSyncService(
            logger.Object,
            syncTasks.Object,
            cache.Object,
            gameManager.Object,
            gameSyncManager.Object,
            syncSourceManager.Object
        );

        return (sut, syncTasks, cache, gameManager, gameSyncManager, syncSourceManager);
    }

    private static SyncSourceEntity CreateLocalSource(string id = "s1")
        => new() { Id = id, StorageProvider = StorageProvider.Dropbox };

    [Fact]
    public async Task TryDetectGameChangesAsync_WhenNoSyncSource_Returns()
    {
        var (sut, _, _, gameManager, _, syncSourceManager) = CreateSut();

        syncSourceManager
            .Setup(x => x.GetLocalAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((SyncSourceEntity?)null);

        await sut.TryDetectGameChangesAsync(CancellationToken.None);

        gameManager.Verify(x => x.GetListAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TryDetectGameChangesAsync_WhenExternalSyncSourceRemoved_ClearsTasksAndUnlinks()
    {
        var (sut, syncTasks, _, _, _, syncSourceManager) = CreateSut();
        var local = CreateLocalSource();

        syncSourceManager
            .Setup(x => x.GetLocalAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(local);

        syncSourceManager
            .Setup(x => x.GetAsync(local.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SyncSourceEntity?)null);

        syncSourceManager
            .Setup(x => x.UnlinkLocalStorageProviderAsync(local, false, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await sut.TryDetectGameChangesAsync(CancellationToken.None);

        syncTasks.Verify(x => x.Clear(), Times.Once);
        syncSourceManager.Verify(
            x => x.UnlinkLocalStorageProviderAsync(local, false, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task TryDetectGameSyncStatusesAsync_UsesGameSyncManagerAndUpdatesCache()
    {
        var (sut, _, cache, _, gameSyncManager, syncSourceManager) = CreateSut();
        var local = CreateLocalSource();

        syncSourceManager
            .Setup(x => x.GetLocalAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(local);

        gameSyncManager
            .Setup(x => x.GetSyncType(local.Id, It.IsAny<GameEntity>()))
            .Returns(new GetSyncTypeResult
            {
                SyncStatus = GameSyncStatus.RequiresUpload
            });

        var games = new List<GameEntity>
        {
            new() { Id = "g1" }
        };

        await sut.TryDetectGameSyncStatusesAsync(games, CancellationToken.None);

        gameSyncManager.Verify(
            x => x.GetSyncType(local.Id, It.IsAny<GameEntity>()),
            Times.Once
        );

        cache.Verify(
            x => x.AddOrUpdate("g1", GameSyncStatus.RequiresUpload),
            Times.Once
        );
    }

    [Fact]
    public async Task TryDetectGameChangesAsync_WhenGameAutoSyncDisabled_RemovesTask()
    {
        var (sut, syncTasks, _, gameManager, _, syncSourceManager) = CreateSut();
        var local = CreateLocalSource();

        syncSourceManager
            .Setup(x => x.GetLocalAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(local);

        syncSourceManager
            .Setup(x => x.GetAsync(local.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(local);

        gameManager
            .Setup(x => x.GetListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GameEntity>
            {
                new() { Id = "g1", AutoSync = false }
            });

        await sut.TryDetectGameChangesAsync(CancellationToken.None);

        syncTasks.Verify(x => x.Remove("g1"), Times.Once);
    }
}