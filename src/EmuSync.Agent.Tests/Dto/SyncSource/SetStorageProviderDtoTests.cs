using EmuSync.Agent.Dto.SyncSource;
using System.Text.Json;

namespace EmuSync.Agent.Tests.Dto.SyncSource;

public class SetStorageProviderDtoTests
{
    [Fact]
    public void DeserialisesCorrectly()
    {
        var json = """
        {
          "storageProviderId": 1
        }
        """;

        var dto = JsonSerializer.Deserialize<SetStorageProviderDto>(json);

        Assert.NotNull(dto);
        Assert.Equal(1, dto.StorageProviderId);
    }
}
