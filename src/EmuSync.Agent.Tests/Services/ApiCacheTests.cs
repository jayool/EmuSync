using EmuSync.Agent.Services;
using EmuSync.Domain.Entities;

namespace EmuSync.Agent.Tests.Services;

public class ApiCacheTests
{
    private static ApiCache CreateSut() => new();

    private static GameEntity CreateGame(
        string id = "g1",
        string name = "G",
        string? lastSyncedFrom = null,
        long storageBytes = 0
    ) => new()
    {
        Id = id,
        Name = name,
        LastSyncedFrom = lastSyncedFrom,
        StorageBytes = storageBytes
    };

    [Fact]
    public void GetGame_WhenPresent_ReturnsGame()
    {
        var sut = CreateSut();
        var game = CreateGame();

        sut.Games.Set(new List<GameEntity> { game });

        var result = sut.GetGame("g1");

        Assert.Equal(game, result);
    }

    [Fact]
    public void UpdateGame_WhenGameExists_UpdatesFields()
    {
        var sut = CreateSut();
        var existing = CreateGame(
            lastSyncedFrom: "a",
            storageBytes: 1
        );

        sut.Games.Set(new List<GameEntity> { existing });

        var updated = new GameEntity
        {
            Id = "g1",
            LastSyncedFrom = "b",
            LastSyncTimeUtc = DateTime.UtcNow,
            LatestWriteTimeUtc = DateTime.UtcNow,
            StorageBytes = 999
        };

        sut.UpdateGame(updated);

        var result = sut.GetGame("g1");

        Assert.NotNull(result);
        Assert.Equal("b", result!.LastSyncedFrom);
        Assert.Equal(999, result.StorageBytes);
        Assert.Equal(updated.LastSyncTimeUtc, result.LastSyncTimeUtc);
        Assert.Equal(updated.LatestWriteTimeUtc, result.LatestWriteTimeUtc);
    }
}