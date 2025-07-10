using System.Net;

namespace HomeBoxLanding.Api.Features.HealthCheck.Types;

public class HealthCheckResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string? StatusDescription { get; set; }
    public long DurationInMilliseconds { get; set; }
}