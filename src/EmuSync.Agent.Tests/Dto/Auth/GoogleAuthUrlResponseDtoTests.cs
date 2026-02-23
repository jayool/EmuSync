using EmuSync.Agent.Dto.Auth;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Auth;

public class GoogleAuthUrlResponseDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new GoogleAuthUrlResponseDto
        {
            Url = "https://x"
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"url\"", json);
    }
}
