using HomeBoxLanding.Api.Core.Shell;
using HomeBoxLanding.Api.Features.Builds.Types;
using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.Builds;

[ApiController]
[Route("api/[controller]")]
public class BuildsController : ControllerBase
{
    private readonly BuildsService _service;

    public BuildsController()
    {
        _service = new BuildsService(ShellService.Instance(), new DockerBuildsRepository());
    }

    [HttpGet("docker-apps")]
    public GetAllDockerBuildsResponse GetAllDockerBuilds()
    {
        return _service.GetAllDockerBuilds();
    }

    [HttpPost("docker-apps")]
    public void UpdateAllDockerApps()
    {
        _service.UpdateAllDockerApps();
    }
}