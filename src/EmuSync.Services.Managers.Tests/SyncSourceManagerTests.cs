using EmuSync.Domain.Entities;
using EmuSync.Domain.Enums;
using EmuSync.Domain.Services.Interfaces;
using EmuSync.Services.Storage.Interfaces;
using EmuSync.Services.Storage.Objects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmuSync.Services.Managers.Tests;

public class SyncSourceManagerTests
{
    private readonly Mock<ILogger<SyncSourceManager>> _logger = new();
    private readonly Mock<ILocalDataAccessor> _local = new();
    private readonly Mock<IStorageProviderFactory> _factory = new();
    private readonly Mock<IStorageProvider> _storage = new();

    private SyncSourceManager CreateSut() => new(
        _logger.Object,
        _local.Object,
        _factory.Object
    );

    [Fact]
    public async Task GetListAsync_Returns_List_WhenFileExists()
    {
        var src = new SyncSource { Id = "s1", Name = "n", OsPlatform = OsPlatform.Windows };
        var file = new SyncSourceListFile { Sources = new List<SyncSource> { src } };

        _storage.Setup(x =>
            x.GetJsonFileAsync<SyncSourceListFile>(It.IsAny<string>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(file);

        _factory.Setup(x =>
            x.CreateAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(_storage.Object);

        var sut = CreateSut();
        var list = await sut.GetListAsync();

        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal("s1", list[0].Id);
    }

    [Fact]
    public async Task GetAsync_Returns_Entity_WhenIdExists()
    {
        var src = new SyncSource { Id = "s1", Name = "n", OsPlatform = OsPlatform.Windows };
        var file = new SyncSourceListFile { Sources = new List<SyncSource> { src } };

        _storage.Setup(x =>
            x.GetJsonFileAsync<SyncSourceListFile>(It.IsAny<string>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(file);

        _factory.Setup(x =>
            x.CreateAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(_storage.Object);

        var sut = CreateSut();
        var entity = await sut.GetAsync("s1");

        Assert.NotNull(entity);
        Assert.Equal("s1", entity.Id);
    }

    [Fact]
    public async Task GetLocalAsync_Returns_Null_WhenFileMissing()
    {
        string returnPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nofile.json");

        _local.Setup(x =>
            x.GetLocalFilePath(It.IsAny<string>())
        ).Returns(returnPath);

        var sut = CreateSut();
        var result = await sut.GetLocalAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateLocalAsync_Returns_Entity_WithId()
    {
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nofile.json");

        _local.Setup(x =>
            x.GetLocalFilePath(It.IsAny<string>())
        ).Returns(tempFile);

        _local.Setup(x =>
            x.WriteFileContentsAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())
        ).Returns(Task.CompletedTask);

        var sut = CreateSut();
        var entity = await sut.CreateLocalAsync();

        Assert.NotNull(entity);
        Assert.False(string.IsNullOrEmpty(entity.Id));
    }

    [Fact]
    public async Task UpdateLocalAsync_Returns_False_When_AutoSyncFrequencyUnchanged()
    {
        var existing = new SyncSourceEntity { Id = "s1", AutoSyncFrequency = TimeSpan.MinValue, Name = "Old" };
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "sync.json");

        _local.Setup(x =>
            x.GetLocalFilePath(It.IsAny<string>())
        ).Returns(tempFile);

        _local.Setup(x =>
            x.ReadFileContentsAsync<SyncSourceEntity>(It.IsAny<string>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(existing);

        _local.Setup(x =>
            x.WriteFileContentsAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())
        ).Returns(Task.CompletedTask);

        var sut = CreateSut();
        var entity = new SyncSourceEntity { Id = "s1", AutoSyncFrequency = TimeSpan.MinValue, Name = "New" };
        var changed = await sut.UpdateLocalAsync(entity);

        Assert.False(changed);
    }

    [Fact]
    public async Task SetLocalStorageProviderAsync_Throws_When_LocalSourceMissing()
    {
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nofile.json");

        _local.Setup(x =>
            x.GetLocalFilePath(It.IsAny<string>())
        ).Returns(tempFile);

        var sut = CreateSut();
        await Assert.ThrowsAsync<NotImplementedException>(async () =>
            await sut.SetLocalStorageProviderAsync(StorageProvider.GoogleDrive)
        );
    }

    [Fact]
    public async Task DeleteAsync_Returns_False_When_IdNotFound()
    {
        _storage.Setup(x =>
            x.GetJsonFileAsync<SyncSourceListFile>(It.IsAny<string>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(new SyncSourceListFile { Sources = new List<SyncSource>() });

        _factory.Setup(x =>
            x.CreateAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(_storage.Object);

        var sut = CreateSut();
        var result = await sut.DeleteAsync("missing");

        Assert.False(result);
    }
}