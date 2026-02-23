using EmuSync.Domain.Entities;
using EmuSync.Domain.Enums;
using EmuSync.Domain.Services;
using EmuSync.Domain.Services.Interfaces;
using Moq;

namespace EmuSync.Domain.Tests.Services;

public class LocalSyncLogTests
{
    private static LocalSyncLog CreateSut(ILocalDataAccessor local) => new(local);

    [Fact]
    public async Task GetAllLogsAsync_WhenNoFile_ReturnsEmpty()
    {
        var local = new Mock<ILocalDataAccessor>();
        local.Setup(x => x.GetLocalFilePath(It.IsAny<string>()))
             .Returns(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "log.log"));

        var sut = CreateSut(local.Object);

        var result = await sut.GetAllLogsAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task WriteLogAsync_WhenCalled_GetAllLogsAsyncReturnsEntry()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var logPath = Path.Combine(tempDir, "log.log");

        var local = new Mock<ILocalDataAccessor>();
        local.Setup(x => x.GetLocalFilePath(It.IsAny<string>())).Returns(logPath);

        var sut = CreateSut(local.Object);

        var entity = new LocalSyncLogEntity
        {
            Id = "id",
            GameId = "g1",
            IsAutoSync = true,
            SyncTimeUtc = DateTime.UtcNow,
            SyncType = SyncType.Upload
        };

        try
        {
            await sut.WriteLogAsync(entity);

            var result = await sut.GetAllLogsAsync();

            Assert.Single(result);
            Assert.Equal("g1", result[0].GameId);
        }
        finally
        {
            try
            {
                if (File.Exists(logPath)) File.Delete(logPath);
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
            catch { }
        }
    }
}