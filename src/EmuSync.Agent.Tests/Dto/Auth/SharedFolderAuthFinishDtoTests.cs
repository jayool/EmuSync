using EmuSync.Agent.Dto.Auth;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Auth;

public class SharedFolderAuthFinishDtoTests
{
    [Fact]
    public void DeserialisesCorrectly()
    {
        var json = """
        {
          "path": "C:/games",
          "username": "u",
          "password": "p"
        }
        """;

        var dto = JsonSerializer.Deserialize<SharedFolderAuthFinishDto>(json);

        Assert.NotNull(dto);
        Assert.Equal("C:/games", dto.Path);
        Assert.Equal("u", dto.Username);
        Assert.Equal("p", dto.Password);
    }
}
