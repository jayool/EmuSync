using EmuSync.Agent.Services;
using EmuSync.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmuSync.Agent.Tests.Services;

public class SyncTasksTests
{
    private static SyncTasks CreateSut()
    {
        var logger = new Mock<ILogger<SyncTasks>>();
        return new SyncTasks(logger.Object);
    }

    private static GameEntity CreateGame(string id = "1", string name = "Game")
        => new() { Id = id, Name = name };

    [Fact]
    public void HasTasks_WhenEmpty_ReturnsFalse()
    {
        var sut = CreateSut();

        var result = sut.HasTasks();

        Assert.False(result);
    }

    [Fact]
    public void HasTasks_WhenTaskAdded_ReturnsTrue()
    {
        var sut = CreateSut();
        sut.Add(CreateGame());

        var result = sut.HasTasks();

        Assert.True(result);
    }

    [Fact]
    public void GetNext_WhenEmpty_ReturnsNull()
    {
        var sut = CreateSut();

        var result = sut.GetNext();

        Assert.Null(result);
    }

    [Fact]
    public void GetNext_WhenTaskExists_ReturnsAndRemovesTask()
    {
        var sut = CreateSut();
        var game = CreateGame();
        sut.Add(game);

        var result = sut.GetNext();

        Assert.NotNull(result);
        Assert.Equal(game.Id, result!.Id);
        Assert.False(sut.HasTasks());
    }

    [Fact]
    public void Add_WithSameId_ReplacesExistingTask()
    {
        var sut = CreateSut();
        sut.Add(CreateGame("1", "Game A"));
        sut.Add(CreateGame("1", "Game B"));

        var result = sut.GetNext();

        Assert.Equal("Game B", result!.Name);
    }

    [Fact]
    public void Update_WhenTaskExists_ReturnsTrue()
    {
        var sut = CreateSut();
        sut.Add(CreateGame("1", "Game"));

        var updated = CreateGame("1", "Updated");

        var result = sut.Update(updated);

        Assert.True(result);
        Assert.Equal("Updated", sut.GetNext()!.Name);
    }

    [Fact]
    public void Update_WhenTaskDoesNotExist_ReturnsFalse()
    {
        var sut = CreateSut();

        var result = sut.Update(CreateGame());

        Assert.False(result);
    }

    [Fact]
    public void Remove_WhenTaskExists_ReturnsTrue()
    {
        var sut = CreateSut();
        var game = CreateGame();
        sut.Add(game);

        var result = sut.Remove(game.Id);

        Assert.True(result);
        Assert.False(sut.HasTasks());
    }

    [Fact]
    public void Remove_WhenTaskDoesNotExist_ReturnsFalse()
    {
        var sut = CreateSut();

        var result = sut.Remove("missing");

        Assert.False(result);
    }

    [Fact]
    public void Clear_RemovesAllTasks()
    {
        var sut = CreateSut();
        sut.Add(CreateGame("1"));
        sut.Add(CreateGame("2"));

        sut.Clear();

        Assert.False(sut.HasTasks());
    }
}