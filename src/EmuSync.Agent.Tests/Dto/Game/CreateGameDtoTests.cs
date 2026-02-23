using EmuSync.Agent.Dto.Game;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Game;

public class CreateGameDtoTests
{
    [Fact]
    public void DeserialisesCorrectly()
    {
        var json = """
        {
          "name": "G",
          "autoSync": false,
          "syncSourceIdLocations": { "s": "p" },
          "maximumLocalGameBackups": 3
        }
        """;

        var dto = JsonSerializer.Deserialize<CreateGameDto>(json);

        Assert.NotNull(dto);
        Assert.Equal("G", dto.Name);
        Assert.False(dto.AutoSync);
        Assert.Equal(3, dto.MaximumLocalGameBackups);

        Assert.NotNull(dto.SyncSourceIdLocations);
        Assert.Equal("p", dto.SyncSourceIdLocations["s"]);
    }
}
