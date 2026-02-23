using EmuSync.Agent.Dto.Auth;
using EmuSync.Domain.Enums;
using EmuSync.Services.Managers.Interfaces;
using EmuSync.Services.Storage.Dropbox;
using EmuSync.Services.Storage.GoogleDrive;
using EmuSync.Services.Storage.OneDrive;
using EmuSync.Services.Storage.SharedFolder;

namespace EmuSync.Agent.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    DropboxAuthHandler dropboxAuthHandler,
    GoogleAuthHandler googleAuthHandler,
    MicrosoftAuthHandler microsoftAuthHandler,
    SharedFolderAuthHandler sharedFolderAuthHandler,
    ISyncSourceManager syncSourceManager,
    IValidationService validator
) : CustomControllerBase(logger, validator)
{
    private readonly DropboxAuthHandler _dropboxAuthHandler = dropboxAuthHandler;
    private readonly GoogleAuthHandler _googleAuthHandler = googleAuthHandler;
    private readonly MicrosoftAuthHandler _microsoftAuthHandler = microsoftAuthHandler;
    private readonly SharedFolderAuthHandler _sharedFolderAuthHandler = sharedFolderAuthHandler;
    private readonly ISyncSourceManager _syncSourceManager = syncSourceManager;

    [HttpGet("Dropbox/AuthUrl")]
    public IActionResult GetDropboxAuthUrl()
    {
        string state = Guid.NewGuid().ToString("N");
        string url = _dropboxAuthHandler.GetAuthUrl(state);

        DropboxAuthUrlResponseDto response = new()
        {
            Url = url,
            State = state
        };

        return Ok(response);
    }

    [HttpGet("Dropbox/AuthFinish")]
    public async Task<IActionResult> DropboxAuthFinish([FromQuery] string code, [FromQuery] string state, CancellationToken cancellationToken)
    {
        await _dropboxAuthHandler.SaveCodeAsync(code, state, cancellationToken);

        StorageProvider storageProvider = StorageProvider.Dropbox;
        await _syncSourceManager.SetLocalStorageProviderAsync(storageProvider, cancellationToken);

        return SuccessfulAuth();
    }

    [HttpGet("Google/AuthUrl")]
    public IActionResult GetGoogleAuthUrl()
    {
        string url = _googleAuthHandler.GetAuthUrl();

        GoogleAuthUrlResponseDto response = new()
        {
            Url = url,
        };

        return Ok(response);
    }

    [HttpGet("Google/AuthFinish")]
    public async Task<IActionResult> GoogleAuthFinish([FromQuery] string code, CancellationToken cancellationToken)
    {
        await _googleAuthHandler.SaveCodeAsync(code, cancellationToken);

        StorageProvider storageProvider = StorageProvider.GoogleDrive;
        await _syncSourceManager.SetLocalStorageProviderAsync(storageProvider, cancellationToken);

        return SuccessfulAuth();
    }

    [HttpGet("Microsoft/AuthUrl")]
    public IActionResult GetMicrosoftAuthUrl()
    {
        string url = _microsoftAuthHandler.GetAuthUrl();

        MicrosoftAuthUrlResponseDto response = new()
        {
            Url = url,
        };

        return Ok(response);
    }

    [HttpGet("Microsoft/AuthFinish")]
    public async Task<IActionResult> MicrosoftAuthFinish([FromQuery] string code, CancellationToken cancellationToken)
    {
        await _microsoftAuthHandler.SaveCodeAsync(code, cancellationToken);

        StorageProvider storageProvider = StorageProvider.OneDrive;
        await _syncSourceManager.SetLocalStorageProviderAsync(storageProvider, cancellationToken);

        return SuccessfulAuth();
    }

    [HttpPost("SharedFolder/AuthFinish")]
    public async Task<IActionResult> SharedFolderAuthFinish([FromBody] SharedFolderAuthFinishDto requestBody, CancellationToken cancellationToken)
    {
        LogRequest($"{nameof(SharedFolderAuthFinish)}", requestBody);

        List<string> bodyErrors = await Validator.ValidateAsync(requestBody, cancellationToken);
        if (bodyErrors.Count > 0) return BadRequestWithErrors(bodyErrors.ToArray());

        var details = requestBody.ToSharedFolderDetails();
        await _sharedFolderAuthHandler.SaveDetailsAsync(details, cancellationToken);

        StorageProvider storageProvider = StorageProvider.SharedFolder;
        await _syncSourceManager.SetLocalStorageProviderAsync(storageProvider, cancellationToken);

        return Ok();
    }

    private IActionResult SuccessfulAuth()
    {
        return Ok("Auth successful - please close this window");
    }
}
