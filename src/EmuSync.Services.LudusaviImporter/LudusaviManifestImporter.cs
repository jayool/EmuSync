using EmuSync.Domain;
using EmuSync.Domain.Services.Interfaces;
using EmuSync.Services.LudusaviImporter.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace EmuSync.Services.LudusaviImporter;

public class LudusaviManifestImporter(
    ILocalDataAccessor localDataAccessor,
    HttpClient httpClient
) : ILudusaviManifestImporter
{
    private readonly ILocalDataAccessor _localDataAccessor = localDataAccessor;

    private const string MANIFEST_URL = "https://raw.githubusercontent.com/mtkennerly/ludusavi-manifest/refs/heads/master/data/manifest.yaml";

    private readonly HttpClient _http = httpClient;

    private static readonly IDeserializer _deserializer =
        new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    public async Task<GameDefinitions?> GetManifestAsync(CancellationToken cancellationToken = default)
    {
        var latestResponse = await DownloadLatestManifestAsync(cancellationToken);

        if (latestResponse.Updated && latestResponse.GameDefinitions != null)
        {
            return latestResponse.GameDefinitions;
        }

        string manifestFileName = GetManifestFileName();

        return await _localDataAccessor.ReadFileContentsOrDefaultAsync<GameDefinitions>(manifestFileName, cancellationToken);
    }

    private async Task<LatestManifestResponse> DownloadLatestManifestAsync(CancellationToken cancellationToken = default)
    {
        string lastEtag = await GetLatestEtagAsync(cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Get, MANIFEST_URL);

        if (!string.IsNullOrWhiteSpace(lastEtag))
        {
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(lastEtag));
        }

        using var response = await _http.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            return new(null, false, null);
        }

        response.EnsureSuccessStatusCode();

        string? latestEtag = response.Headers.ETag?.Tag;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        var data = _deserializer.Deserialize<Dictionary<string, GameDefinition>>(reader);

        var definitions = new GameDefinitions()
        {
            Items = data.Where(x => (x.Value.Files?.Keys.Count ?? 0) > 0).ToDictionary()
        };

        var latestManifestResponse = new LatestManifestResponse(definitions, true, latestEtag);

        await SaveNewManifestDetailsAsync(latestManifestResponse, cancellationToken);

        return latestManifestResponse;
    }

    private async Task SaveNewManifestDetailsAsync(LatestManifestResponse latestManifestResponse, CancellationToken cancellationToken)
    {
        if (latestManifestResponse.GameDefinitions != null)
        {
            string manifestFileName = GetManifestFileName();
            await _localDataAccessor.WriteFileContentsAsync(manifestFileName, latestManifestResponse.GameDefinitions, cancellationToken);
        }

        string latestEtagFileName = GetLatestEtagFileName();
        LatestEtag etag = new(latestManifestResponse.LatestEtag);
        await _localDataAccessor.WriteFileContentsAsync(latestEtagFileName, etag, cancellationToken);
    }

    private async Task<string> GetLatestEtagAsync(CancellationToken cancellationToken)
    {
        string latestEtagFileName = GetLatestEtagFileName();
        var latestEtag = await _localDataAccessor.ReadFileContentsOrDefaultAsync<LatestEtag>(latestEtagFileName, cancellationToken);
        return latestEtag?.eTag ?? "";
    }

    private string GetLatestEtagFileName()
    {
        return _localDataAccessor.GetLocalFilePath(DomainConstants.LocalDataLudusaviLastEtagFile);
    }

    private string GetManifestFileName()
    {
        return _localDataAccessor.GetLocalFilePath(DomainConstants.LocalDataLudusaviManifestFile);
    }
}

public record LatestManifestResponse(
    GameDefinitions? GameDefinitions,
    bool Updated,
    string? LatestEtag
);

public record LatestEtag(
    string? eTag
);