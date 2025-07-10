using HomeBoxLanding.Api.Core.Types;

namespace HomeBoxLanding.Api.Features.Builds.Types;

public class SaveDockerBuildResponse : CommunicationResponse
{
    public Guid DockerBuildIdentifier { get; set; }
}