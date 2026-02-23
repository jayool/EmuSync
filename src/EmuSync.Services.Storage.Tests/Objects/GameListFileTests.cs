using EmuSync.Services.Storage.Objects;
using System.Text.Json;

namespace EmuSync.Services.Storage.Tests.Objects;

public class GameListFileTests
{
    [Fact]
    public void SerializesCorrectly()
    {
        var obj = new GameListFile
        {
            Games = new List<GameMetaData>
            {
                new GameMetaData { Id = "g1", Name = "N" }
            }
        };

        var json = JsonSerializer.Serialize(obj);

        Assert.Contains("\"games\"", json);
    }

    [Fact]
    public void DeserializesCorrectly()
    {
        var json = """
        {
          "games": [
            { "id": "g1", "b": "N", "as": false }
          ]
        }
        """;

        var obj = JsonSerializer.Deserialize<GameListFile>(json);

        Assert.NotNull(obj);
        Assert.Single(obj.Games);
        Assert.Equal("g1", obj.Games[0].Id);
        Assert.Equal("N", obj.Games[0].Name);
        Assert.False(obj.Games[0].AutoSync);
    }
}