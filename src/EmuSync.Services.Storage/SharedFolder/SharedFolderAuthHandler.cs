using EmuSync.Domain.Services.Interfaces;

namespace EmuSync.Services.Storage.SharedFolder;

public class SharedFolderAuthHandler(
    ILocalDataAccessor localDataAccessor
)
{
    private readonly ILocalDataAccessor _localDataAccessor = localDataAccessor;

    private const string JsonFolderName = "shared-folder";
    private const string JsonFileName = "shared-folder.json";

    public async Task SaveDetailsAsync(SharedFolderDetails details, CancellationToken cancellationToken = default)
    {
        string localFilePath = GetJsonFilePath();
        await _localDataAccessor.WriteFileContentsAsync(localFilePath, details, cancellationToken);
    }

    public async Task<SharedFolderDetails> GetDetailsAsync(CancellationToken cancellationToken = default)
    {
        string localFilePath = GetJsonFilePath();
        return await _localDataAccessor.ReadFileContentsAsync<SharedFolderDetails>(localFilePath, cancellationToken);
    }

    public void RemoveJson()
    {
        string localFilePath = GetJsonFilePath();
        _localDataAccessor.RemoveFile(localFilePath);
    }

    private string GetJsonFilePath()
    {
        string path = Path.Combine(JsonFolderName, JsonFileName);
        return _localDataAccessor.GetLocalFilePath(path);
    }
}
