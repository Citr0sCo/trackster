using System.ComponentModel.DataAnnotations;

namespace HomeBoxLanding.Api.Features.Builds.Types;

public class DockerBuildRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? Log { get; set; }
}