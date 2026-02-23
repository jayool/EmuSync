using System;
using System.Collections.Generic;
using System.Text.Json;
using EmuSync.Services.Storage.Objects;
using Xunit;

namespace EmuSync.Services.Storage.Tests.Objects;

public class GameMetaDataTests
{
    [Fact]
    public void SerializesCorrectly()
    {
        var now = DateTime.UtcNow;

        var obj = new GameMetaData
        {
            Id = "g1",
            Name = "N",
            AutoSync = true,
            SyncSourceIdLocations = new Dictionary<string, string> { { "s", "p" } },
            LastSyncedFrom = "x",
            LastSyncTimeUtc = now,
            LatestWriteTimeUtc = now,
            StorageBytes = 10,
            MaximumLocalGameBackups = 2
        };

        var json = JsonSerializer.Serialize(obj);

        Assert.Contains("\"id\"", json);
        Assert.Contains("\"b\"", json); // short name property
    }

    [Fact]
    public void DeserializesCorrectly()
    {
        var json = """
        {
          "id": "g1",
          "b": "N",
          "as": true,
          "sl": { "s": "p" },
          "lsf": "x",
          "lst": "2020-01-01T00:00:00Z",
          "lwt": "2020-01-02T00:00:00Z",
          "sb": 10,
          "mlgb": 2
        }
        """;

        var obj = JsonSerializer.Deserialize<GameMetaData>(json);

        Assert.NotNull(obj);
        Assert.Equal("g1", obj.Id);
        Assert.Equal("N", obj.Name);
        Assert.True(obj.AutoSync);
        Assert.Equal(10, obj.StorageBytes);
    }
}