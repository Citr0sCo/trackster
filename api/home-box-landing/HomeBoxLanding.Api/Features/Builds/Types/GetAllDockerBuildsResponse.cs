namespace HomeBoxLanding.Api.Features.Builds.Types;

public class GetAllDockerBuildsResponse
{
    public GetAllDockerBuildsResponse()
    {
        DockerBuild = new List<DockerBuild>();
    }
        
    public List<DockerBuild> DockerBuild { get; set; }
}