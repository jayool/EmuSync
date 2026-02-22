namespace EmuSync.Services.Managers.Objects;

public record GameBulkUpsert
{
    public string? ExistingGameId { get; set; }
    public string Path { get; set; }
    public string? GameName { get; set; }
    public bool? AutoSync { get; set; }
    public int? MaximumLocalGameBackups { get; set; }
}
