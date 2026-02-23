using EmuSync.Agent.Services;
using EmuSync.Agent.Services.Interfaces;
using EmuSync.Domain.Entities;
using EmuSync.Domain.Enums;
using EmuSync.Services.Managers.Interfaces;
using EmuSync.Services.Managers.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmuSync.Agent.Tests.Services;

public class SyncTaskProcessorTests
{
    private static (
        SyncTaskProcessor Sut,
        Mock<IGameSyncStatusCache> Cache,
        Mock<IGameSyncManager> GameSyncManager,
        Mock<ISyncSourceManager> SyncSourceManager,
        Mock<IApiCache> ApiCache
    ) CreateSut()
    {
        var logger = new Mock<ILogger<SyncTaskProcessor>>();
        var cache = new Mock<IGameSyncStatusCache>();
        var gameSyncManager = new Mock<IGameSyncManager>();
        var syncSourceManager = new Mock<ISyncSourceManager>();
        var apiCache = new Mock<IApiCache>();

        var sut = new SyncTaskProcessor(
            logger.Object,
            cache.Object,
            gameSyncManager.Object,
            syncSourceManager.Object,
            apiCache.Object
        );

        return (sut, cache, gameSyncManager, syncSourceManager, apiCache);
    }

    private static GameEntity CreateGame(string id = "g1", string name = "G")
        => new() { Id = id, Name = name };

    private static SyncSourceEntity CreateLocalSource(
        string id = "s1",
        StorageProvider? provider = StorageProvider.Dropbox
    ) => new() { Id = id, StorageProvider = provider };

    [Fact]
    public async Task ProcessSyncTaskAsync_WhenNoSyncSource_Returns()
    {
        var (sut, _, gameSyncManager, syncSourceManager, apiCache) = CreateSut();

        syncSourceManager
            .Setup(x => x.GetLocalAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((SyncSourceEntity?)null);

        await sut.ProcessSyncTaskAsync(CreateGame(), CancellationToken.None);

        gameSyncManager.Verify(
            x => x.ForceDownloadGameAsync(
                It.IsAny<string>(),
                It.IsAny<GameEntity>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );

        apiCache.Verify(x => x.UpdateGame(It.IsAny<GameEntity>()), Times.Never);
    }

    [Fact]
    public async Task ProcessSyncTaskAsync_WhenNoStorageProvider_Returns()
    {
        var (sut, _, gameSyncManager, syncSourceManager, apiCache) = CreateSut();

        syncSourceManager
            .Setup(x => x.GetLocalAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateLocalSource(provider: null));

        await sut.ProcessSyncTaskAsync(CreateGame(), CancellationToken.None);

        gameSyncManager.Verify(
            x => x.ForceDownloadGameAsync(
                It.IsAny<string>(),
                It.IsAny<GameEntity>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );

        apiCache.Verify(x => x.UpdateGame(It.IsAny<GameEntity>()), Times.Never);
    }

    [Fact]
    public async Task ProcessSyncTaskAsync_WhenRequiresDownload_ProcessesAndUpdatesCaches()
    {
        var (sut, cache, gameSyncManager, syncSourceManager, apiCache) = CreateSut();
        var source = CreateLocalSource();
        var game = CreateGame();

        syncSourceManager
            .Setup(x => x.GetLocalAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        gameSyncManager
            .Setup(x => x.GetSyncType(source.Id, It.IsAny<GameEntity>()))
            .Returns(new GetSyncTypeResult
            {
                SyncStatus = GameSyncStatus.RequiresDownload
            });

        gameSyncManager
            .Setup(x => x.ForceDownloadGameAsync(
                source.Id,
                game,
                true,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await sut.ProcessSyncTaskAsync(game, CancellationToken.None);

        gameSyncManager.Verify(
            x => x.ForceDownloadGameAsync(source.Id, game, true, It.IsAny<CancellationToken>()),
            Times.Once
        );

        cache.Verify(
            x => x.AddOrUpdate(game.Id, GameSyncStatus.InSync),
            Times.Once
        );

        apiCache.Verify(x => x.UpdateGame(game), Times.Once);
    }
}