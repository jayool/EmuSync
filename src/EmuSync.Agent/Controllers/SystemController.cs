namespace EmuSync.Agent.Controllers;

[ApiController]
[Route("[controller]")]
public class SystemController(
    ILogger<SystemController> logger,
    IValidationService validator
) : CustomControllerBase(logger, validator)
{
    [HttpGet("HealthCheck")]
    [HttpPost("HealthCheck")]
    public async Task<IActionResult> HealthCheck(CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}
