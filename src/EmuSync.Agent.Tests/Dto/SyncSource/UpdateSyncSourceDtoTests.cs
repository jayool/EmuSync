using EmuSync.Agent.Dto.SyncSource;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.SyncSource;

public class UpdateSyncSourceDtoTests
{
    [Fact]
    public void DeserialisesCorrectly()
    {
        var json = """
        {
          "name": "n",
          "autoSyncFrequencyMins": 30,
          "maximumLocalGameBackups": 2
        }
        """;

        var dto = JsonSerializer.Deserialize<UpdateSyncSourceDto>(json);

        Assert.NotNull(dto);
        Assert.Equal("n", dto.Name);
        Assert.Equal(30, dto.AutoSyncFrequencyMins);
        Assert.Equal(2, dto.MaximumLocalGameBackups);
    }
}
