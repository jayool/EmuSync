using EmuSync.Agent.Dto.Game;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Game;

public class QuickAddRequestBodyDtoTests
{
    [Fact]
    public void DeserialisesCorrectly()
    {
        var json = """
        {
          "games": [
            {
              "existingGameId": null,
              "path": "C:/g",
              "gameName": "Name",
              "autoSync": true,
              "maximumLocalGameBackups": 5
            }
          ]
        }
        """;

        var dto = JsonSerializer.Deserialize<QuickAddRequestBodyDto>(json);

        Assert.NotNull(dto);
        Assert.Single(dto.Games);

        var g = dto.Games[0];
        Assert.Null(g.ExistingGameId);
        Assert.Equal("C:/g", g.Path);
        Assert.Equal("Name", g.GameName);
        Assert.True(g.AutoSync);
        Assert.Equal(5, g.MaximumLocalGameBackups);
    }
}
