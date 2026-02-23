using EmuSync.Agent.Dto.Auth;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Auth;

public class DropboxAuthUrlResponseDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new DropboxAuthUrlResponseDto
        {
            Url = "https://x",
            State = "s"
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"url\"", json);
        Assert.Contains("\"state\"", json);
    }
}
