using EmuSync.Domain.Entities;

namespace EmuSync.Domain.Services.Interfaces;

public interface ILocalGameSaveBackupService
{
    Task<List<LocalGameBackupManifestEntity>> GetBackupsAsync(string gameId, CancellationToken cancellationToken = default);
    Task CreateBackupAsync(string gameId, string path, Action<double>? onProgress = null, CancellationToken cancellationToken = default);
    Task DeleteBackupAsync(string gameId, string backupId, CancellationToken cancellationToken = default);
    Task RestoreBackupAsync(string gameId, string backupFileName, string outputDirectory, CancellationToken cancellationToken = default);
}