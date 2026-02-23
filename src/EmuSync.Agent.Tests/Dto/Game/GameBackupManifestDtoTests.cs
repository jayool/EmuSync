using EmuSync.Agent.Dto.Game;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Game;

public class GameBackupManifestDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new GameBackupManifestDto
        {
            Id = "i",
            BackupFileName = "f",
            CreatedOnUtc = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"id\"", json);
        Assert.Contains("\"backupFileName\"", json);
        Assert.Contains("\"createdOnUtc\"", json);
    }
}
