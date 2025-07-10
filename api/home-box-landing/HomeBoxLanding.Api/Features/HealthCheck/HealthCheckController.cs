using HomeBoxLanding.Api.Features.HealthCheck.Types;
using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.HealthCheck;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly HealthCheckService _service;

    public HealthCheckController(IHttpClientFactory httpClientFactory)
    {
        _service = new HealthCheckService(httpClientFactory);
    }

    [HttpGet]
    public async Task<HealthCheckResponse> Get([FromQuery] string url, [FromQuery] bool isSecure)
    {
        return await _service.PerformHealthCheck(url, isSecure).ConfigureAwait(false);
    }
}