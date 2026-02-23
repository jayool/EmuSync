using EmuSync.Agent.Dto.Common;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.Common;

public class ErrorResponseDtoTests
{
    [Fact]
    public void SerialisesCorrectly()
    {
        var dto = new ErrorResponseDto
        {
            Status = 400,
            Title = "Bad",
            Errors = new List<string> { "e" }
        };

        var json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"status\"", json);
        Assert.Contains("\"title\"", json);
        Assert.Contains("\"errors\"", json);
    }
}
