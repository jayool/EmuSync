using EmuSync.Agent.Dto.Game;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Game;

public class GameSuggestionDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new GameSuggestionDto
        {
            Name = "n",
            SuggestedFolderPaths = new List<string> { "p" }
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"name\"", json);
        Assert.Contains("\"suggestedFolderPaths\"", json);
    }
}
