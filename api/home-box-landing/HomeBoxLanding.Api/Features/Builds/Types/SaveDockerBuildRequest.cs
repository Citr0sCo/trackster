namespace HomeBoxLanding.Api.Features.Builds.Types;

public class SaveDockerBuildRequest
{
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? Log { get; set; }
}