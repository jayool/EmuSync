using Dropbox.Api;
using Dropbox.Api.Files;
using EmuSync.Domain.Objects;
using EmuSync.Services.Storage.Interfaces;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace EmuSync.Services.Storage.Dropbox;

public class DropboxStorageProvider(
    IOptions<DropboxStorageProviderConfig> options,
    DropboxAuthHandler authHandler
) : IStorageProvider
{
    private readonly DropboxStorageProviderConfig _options = options.Value;
    private readonly DropboxAuthHandler _authHandler = authHandler;
    private DropboxClient _dropboxClient;

    public async Task<TData?> GetJsonFileAsync<TData>(string fileName, CancellationToken cancellationToken = default)
    {
        string fullFilePath = GetFullFilePath(fileName);
        var client = await GetDropboxClientAsync(cancellationToken);

        try
        {
            using var response = await client.Files.DownloadAsync(fullFilePath);
            using var json = await response.GetContentAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<TData>(json, cancellationToken: cancellationToken)!;
        }
        catch (ApiException<DownloadError> downloadError)
        {
            //if the item wasn't found, that's fine, otherwise throw the error
            if (!downloadError.Message.Contains("/not_found"))
            {
                throw;
            }

            return default;
        }
        catch (Exception)
        {
            //throw everything else
            throw;
        }
    }

    public async Task GetZipFileAsync(string fileName, string writeToPath, Action<double>? onProgress = null, CancellationToken cancellationToken = default)
    {
        string fullFilePath = GetFullFilePath(fileName);

        string path = Path.GetDirectoryName(writeToPath)!;

        if (!Path.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var client = await GetDropboxClientAsync(cancellationToken);
        using var response = await client.Files.DownloadAsync(fullFilePath);

        var totalSize = response.Response.Size;

        using var stream = await response.GetContentAsStreamAsync();
        using var fileStream = new FileStream(writeToPath, FileMode.CreateNew, FileAccess.Write);
        using var progressStream = new ProgressStream(fileStream, onProgress, totalSize);

        //copy the content in chunks, reporting progress
        byte[] buffer = new byte[81920]; // 80KB
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            await progressStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
        }
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        string fullFilePath = GetFullFilePath(fileName);
        var client = await GetDropboxClientAsync(cancellationToken);

        try
        {
            await client.Files.DeleteV2Async(fullFilePath);
        }
        catch (ApiException<DeleteError> ex)
        {
            //if the item wasn't found, that's fine, otherwise throw the error
            if (!ex.Message.Contains("/not_found"))
            {
                throw;
            }
        }

    }

    public async Task UpsertJsonDataAsync(
        string fileName,
        object data,
        Action<double>? onProgress = null,
        CancellationToken cancellationToken = default
    )
    {
        string fullFilePath = GetFullFilePath(fileName);

        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(data, new JsonSerializerOptions { WriteIndented = false });

        using var stream = new MemoryStream(bytes);
        using var progressStream = new ProgressStream(stream, onProgress);

        await UpsertStreamAsSessionAsync(fullFilePath, stream, onProgress, cancellationToken);
    }

    public async Task UpsertZipDataAsync(
        string fileName,
        Stream stream,
        Action<double>? onProgress = null,
        CancellationToken cancellationToken = default
    )
    {
        string fullFilePath = GetFullFilePath(fileName);
        await UpsertStreamAsSessionAsync(fullFilePath, stream, onProgress, cancellationToken);
    }

    public void RemoveRelatedFiles()
    {
        _authHandler.RemoveToken();
    }

    /// <summary>
    /// Creates or updates a zip file
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<FileMetadata> UpsertStreamAsSessionAsync(
        string fileName,
        Stream stream,
        Action<double>? onProgress = null,
        CancellationToken cancellationToken = default
    )
    {
        using var progressStream = new ProgressStream(stream, onProgress);

        const int chunkSize = 5 * 1024 * 1024; //5MB
        var client = await GetDropboxClientAsync(cancellationToken);

        ulong uploaded = 0;
        var buffer = new byte[chunkSize];

        // Start session
        var read = await progressStream.ReadAsync(buffer, cancellationToken);
        using var firstChunk = new MemoryStream(buffer, 0, read);

        var session = await client.Files.UploadSessionStartAsync(
            body: firstChunk
        );

        uploaded += (ulong)read;

        //append
        while ((read = await progressStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            using var chunk = new MemoryStream(buffer, 0, read);

            await client.Files.UploadSessionAppendV2Async(
                new UploadSessionCursor(session.SessionId, uploaded),
                body: chunk
            );

            uploaded += (ulong)read;
        }

        //finish
        return await client.Files.UploadSessionFinishAsync(
            new UploadSessionCursor(session.SessionId, uploaded),
            new CommitInfo(fileName, mode: WriteMode.Overwrite.Instance),
            body: progressStream
        );
    }

    /// <summary>
    /// Connects to the drive service
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<DropboxClient> GetDropboxClientAsync(CancellationToken cancellationToken = default)
    {
        if (_dropboxClient != null) return _dropboxClient;

        var token = await _authHandler.GetTokenAsync(cancellationToken);

        if (token == null)
        {
            throw new InvalidOperationException("No dropbox token is present on the device");
        }

        //dropbox refresh tokens don't expire
        DropboxClient client = new(token.RefreshToken, appKey: _options.AppKey, config: new DropboxClientConfig()
        {
            HttpClient = new()
            {
                Timeout = Timeout.InfiniteTimeSpan
            }
        });

        _dropboxClient = client;
        return _dropboxClient;
    }

    private string GetFullFilePath(string fileName)
    {
        return "/" + fileName;
    }
}
