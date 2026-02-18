using EmuSync.Domain.Enums;
using EmuSync.Domain.Helpers;
using EmuSync.Domain.Objects;
using EmuSync.Services.Storage.Interfaces;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EmuSync.Services.Storage.SharedFolder;

public class SharedFolderStorageProvider(
    SharedFolderAuthHandler authHandler
) : IStorageProvider
{
    private static readonly bool _isWindows = PlatformHelper.GetOsPlatform() == OsPlatform.Windows;
    private readonly SharedFolderAuthHandler _authHandler = authHandler;

    public async Task<TData?> GetJsonFileAsync<TData>(string fileName, CancellationToken cancellationToken = default)
    {
        string fullFilePath = await GetFullFilePathAsync(fileName, cancellationToken);
        using FileStream? content = await GetSharedFileStreamAsync(fullFilePath, cancellationToken);

        if (content == null)
        {
            return default;
        }

        return await JsonSerializer.DeserializeAsync<TData>(content, cancellationToken: cancellationToken);
    }

    public async Task GetZipFileAsync(string fileName, string writeToPath, Action<double>? onProgress = null, CancellationToken cancellationToken = default)
    {
        string path = Path.GetDirectoryName(writeToPath)!;

        if (!Path.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string fullFilePath = await GetFullFilePathAsync(fileName, cancellationToken);
        using FileStream? stream = await GetSharedFileStreamAsync(fullFilePath, cancellationToken);

        if (stream == null)
        {
            throw new Exception("No stream exists");
        }

        using var fileStream = new FileStream(writeToPath, FileMode.CreateNew, FileAccess.Write);

        ulong totalSize = stream.Length > 0 ? (ulong)stream.Length : 0;
        using var progressStream = new ProgressStream(fileStream, onProgress, totalSize);

        //copy the content in chunks, reporting progress
        byte[] buffer = new byte[81920]; // 80KB
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await progressStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
        }
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        string fullFilePath = await GetFullFilePathAsync(fileName, cancellationToken);
        await DeleteSharedFileAsync(fullFilePath, cancellationToken);
    }

    public async Task UpsertJsonDataAsync(
        string fileName,
        object data,
        Action<double>? onProgress = null,
        CancellationToken cancellationToken = default
    )
    {

        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(data, new JsonSerializerOptions { WriteIndented = false });

        string fullFilePath = await GetFullFilePathAsync(fileName, cancellationToken);
        await WriteSharedFileContentBytesAsync(fullFilePath, bytes, cancellationToken);
    }

    public async Task UpsertZipDataAsync(
        string fileName,
        Stream stream,
        Action<double>? onProgress = null,
        CancellationToken cancellationToken = default
    )
    {
        string fullFilePath = await GetFullFilePathAsync(fileName, cancellationToken);

        stream.Position = 0;
        using var progressStream = new ProgressStream(stream, onProgress);

        await WriteSharedFileContentStreamAsync(fullFilePath, progressStream, cancellationToken);
    }

    public void RemoveRelatedFiles()
    {
        //_sharedFolderDetails = null;
        _authHandler.RemoveJson();
    }

    private async Task<FileStream?> GetSharedFileStreamAsync(string fullFilePath, CancellationToken cancellationToken)
    {
        FileStream? Read()
        {
            if (File.Exists(fullFilePath))
            {
                return File.OpenRead(fullFilePath);
            }

            return null;
        }

        if (_isWindows)
        {
            var details = await GetSharedFolderDetailsAsync(cancellationToken);

            if (details.IsWindowsShared())
            {
                using (var connection = new NetworkConnection(details))
                {
                    return Read();
                }
            }
        }

        return Read();
    }

    private async Task DeleteSharedFileAsync(string fullFilePath, CancellationToken cancellationToken)
    {
        void Delete()
        {
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }
        }

        if (_isWindows)
        {
            var details = await GetSharedFolderDetailsAsync(cancellationToken);

            if (details.IsWindowsShared())
            {
                using (var connection = new NetworkConnection(details))
                {
                    Delete();
                    return;
                }
            }
        }

        Delete();
    }

    private async Task WriteSharedFileContentBytesAsync(string fullFilePath, byte[] bytes, CancellationToken cancellationToken)
    {
        async Task Write()
        {
            string dir = Path.GetDirectoryName(fullFilePath)!;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllBytesAsync(fullFilePath, bytes, cancellationToken);
        }

        if (_isWindows)
        {
            var details = await GetSharedFolderDetailsAsync(cancellationToken);

            if (details.IsWindowsShared())
            {
                using (var connection = new NetworkConnection(details))
                {
                    await Write();
                    return;
                }
            }
        }

        await Write();
    }

    private async Task WriteSharedFileContentStreamAsync(string fullFilePath, Stream stream, CancellationToken cancellationToken)
    {
        async Task Write()
        {
            using var fileStream = File.OpenWrite(fullFilePath);
            await stream.CopyToAsync(fileStream, cancellationToken);
        }

        if (_isWindows)
        {
            var details = await GetSharedFolderDetailsAsync(cancellationToken);

            if (details.IsWindowsShared())
            {
                using (var connection = new NetworkConnection(details))
                {
                    await Write();
                    return;
                }
            }
        }

        await Write();
    }

    private async Task<string> GetFullFilePathAsync(string path, CancellationToken cancellationToken)
    {
        var details = await GetSharedFolderDetailsAsync(cancellationToken);
        return Path.Combine(details.Path, path);
    }

    private async Task<SharedFolderDetails> GetSharedFolderDetailsAsync(CancellationToken cancellationToken)
    {
        return await _authHandler.GetDetailsAsync(cancellationToken);
        //if (_sharedFolderDetails == null)
        //{
        //    _sharedFolderDetails = await _authHandler.GetDetailsAsync(cancellationToken);
        //}

        //return _sharedFolderDetails;
    }
}