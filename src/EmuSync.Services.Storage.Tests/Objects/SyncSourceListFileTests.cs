using System.Text.Json;
using EmuSync.Domain.Enums;
using EmuSync.Services.Storage.Objects;
using Xunit;

namespace EmuSync.Services.Storage.Tests.Objects;

public class SyncSourceListFileTests
{
    [Fact]
    public void SerializesCorrectly()
    {
        var src = new SyncSource
        {
            Id = "s1",
            Name = "n",
            OsPlatform = OsPlatform.Windows
        };

        var file = new SyncSourceListFile
        {
            Sources = new List<SyncSource> { src }
        };

        var json = JsonSerializer.Serialize(file);

        Assert.Contains("\"sources\"", json);
    }

    [Fact]
    public void DeserializesCorrectly()
    {
        var json = """
        {
          "sources": [
            { "id": "s1", "name": "n", "storageProvider": 2, "osPlatform": 1 }
          ]
        }
        """;

        var file = JsonSerializer.Deserialize<SyncSourceListFile>(json);

        Assert.NotNull(file);
        Assert.Single(file.Sources);
        Assert.Equal("s1", file.Sources[0].Id);
    }
}