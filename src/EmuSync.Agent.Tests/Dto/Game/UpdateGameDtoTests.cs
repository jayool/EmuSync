using EmuSync.Agent.Dto.Game;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Game;

public class UpdateGameDtoTests
{
    [Fact]
    public void DeserialisesCorrectly()
    {
        var json = """
        {
          "id": "i",
          "name": "n",
          "autoSync": true,
          "syncSourceIdLocations": { "s": "p" },
          "maximumLocalGameBackups": 4
        }
        """;

        var dto = JsonSerializer.Deserialize<UpdateGameDto>(json);

        Assert.NotNull(dto);
        Assert.Equal("i", dto.Id);
        Assert.Equal("n", dto.Name);
        Assert.True(dto.AutoSync);
        Assert.Equal(4, dto.MaximumLocalGameBackups);
    }
}
