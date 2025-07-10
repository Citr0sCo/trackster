using HomeBoxLanding.Api.Core.Shell;
using HomeBoxLanding.Api.Features.Builds.Types;
using HomeBoxLanding.Api.Features.WebSockets.Types;

namespace HomeBoxLanding.Api.Features.Builds;

public class BuildsService
{
    private readonly IShellService _shellService;
    private readonly IDockerBuildsRepository _dockerBuildsRepository;

    public BuildsService(IShellService shellService, IDockerBuildsRepository dockerBuildsRepository)
    {
        _shellService = shellService;
        _dockerBuildsRepository = dockerBuildsRepository;
    }

    public GetAllDockerBuildsResponse GetAllDockerBuilds()
    {
        var builds = _dockerBuildsRepository.GetAll();

        return new GetAllDockerBuildsResponse
        {
            DockerBuild = builds.ConvertAll(x => new DockerBuild
            {
                Identifier = x.Identifier,
                FinishedAt = x.FinishedAt,
                StartedAt = x.StartedAt,
                Log = x.Log
            })
        };
    }
    
    public void UpdateAllDockerApps()
    {
        var rootFolder = Environment.GetEnvironmentVariable("ASPNETCORE_UPDATE_SCRIPT_ROOT");
        
        var logFile = _shellService.Run("echo output_$(date +%Y-%m-%d-%H-%M).log").TrimEnd(Environment.NewLine.ToCharArray());
        _shellService.RunOnHost($"touch {rootFolder}/{logFile}");
        
        Thread.Sleep(1000);
        
        _shellService.RunOnHost($"bash {rootFolder}/update-all-via-web.sh >> {rootFolder}/{logFile} 2>&1");

        var logPath = $"/host/tools/updater/{logFile}";
        var output = "";

        var startTime = DateTime.Now;
        
        while (output.Contains("DONE!") is false)
        {
            Console.WriteLine($"Checking if file exists at {logPath}...");
            
            if (File.Exists(logPath) is false)
            {
                Console.WriteLine("File doesn't exist. Sleeping for 1s...");
                Thread.Sleep(1000);
                continue;
            }
            
            Console.WriteLine("File exists. Reading content...");
            
            output = File.ReadAllTextAsync(logPath).Result;
            
            Console.WriteLine("File read. Streaming to web socket clients...");
            
            WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.DockerAppUpdateProgress,  new
            {
                Result = output,
                Finished = false
            });
            
            Console.WriteLine("File not finished. Sleeping for 1s...");
            Thread.Sleep(1000);
        }

        _dockerBuildsRepository.SaveBuild(new SaveDockerBuildRequest
        {
            StartedAt = startTime,
            FinishedAt = DateTime.Now,
            Log = output
        });
            
        WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.DockerAppUpdateProgress, new
        {
            Response = new
            {
                Data = new
                {
                    Result = output,
                    Finished = true
                }
            }
        });
    }
}