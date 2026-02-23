using EmuSync.Domain.Enums;
using EmuSync.Services.Managers.Results;

namespace EmuSync.Services.Managers.Tests.Results;

public class GetSyncTypeResultTests
{
    [Fact]
    public void NoLocalFolderPath_ReturnsTrue_WhenFolderPathIsNullOrEmpty()
    {
        var result = new GetSyncTypeResult { SyncStatus = GameSyncStatus.Unknown, FolderPath = "" };
        Assert.True(result.NoLocalFolderPath);

        result = new GetSyncTypeResult { FolderPath = "" };
        Assert.True(result.NoLocalFolderPath);

        result = new GetSyncTypeResult { FolderPath = "p" };
        Assert.False(result.NoLocalFolderPath);
    }
}