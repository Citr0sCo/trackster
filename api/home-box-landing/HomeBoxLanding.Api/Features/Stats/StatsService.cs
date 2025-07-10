using HomeBoxLanding.Api.Core.Events.Types;
using HomeBoxLanding.Api.Core.Shell;
using HomeBoxLanding.Api.Core.Types;
using HomeBoxLanding.Api.Features.Stats.Types;
using HomeBoxLanding.Api.Features.WebSockets.Types;
using Newtonsoft.Json;
using WebSocketManager = HomeBoxLanding.Api.Features.WebSockets.WebSocketManager;

namespace HomeBoxLanding.Api.Features.Stats;

public class StatsService : ISubscriber
{
    private readonly IStatsServiceCache _cacheService;
    private readonly IShellService _shellService;
    private bool _isStarted;

    public StatsService(IShellService shellService, IStatsServiceCache cacheService)
    {
        _shellService = shellService;
        _cacheService = cacheService;
    }

    public void OnStarted()
    {
        _isStarted = true;

        Task.Run(() =>
        {
            while (_isStarted)
            {
                WebSocketManager.Instance().SendToAllClients(WebSocketKey.ServerStats, GetServerStats(true));

                Thread.Sleep(15000);
            }
        }, CancellationToken.None);
    }

    public void OnStopping()
    {
        _isStarted = false;
    }

    public void OnStopped()
    {
        // Do nothing
    }

    public StatsResponse GetServerStats(bool forceCheck = false)
    {
        if (_cacheService.GetStats() != null && !forceCheck)
            return _cacheService.GetStats() ?? new StatsResponse();

        var response = new StatsResponse();

        var output = string.Empty;

        try
        {
            output = _shellService.RunOnHost("docker stats --no-stream");
        }
        catch (Exception)
        {
            return new StatsResponse
            {
                HasError = true,
                Error = new Error
                {
                    Code = ErrorCode.FailedToGetStats,
                    UserMessage = "Failed to run shell command.",
                    TechnicalMessage = $"Received the following: {output}"
                }
            };
        }

        var lines = output.Split("\n");

        if (lines.Length < 2)
            return new StatsResponse
            {
                HasError = true,
                Error = new Error
                {
                    Code = ErrorCode.FailedToGetStats,
                    UserMessage = "Incorrect number of lines received from shell",
                    TechnicalMessage = $"Received the following: {JsonConvert.SerializeObject(lines)}"
                }
            };

        foreach (var line in lines)
        {
            var stats = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (stats.Length == 0)
                continue;
            
            if(stats[0] == "CONTAINER")
                continue;
            
            if(stats.Any((x) => x == "Executing command"))
                continue;
            
            var driveInfo = new DriveInfo(AppContext.BaseDirectory);
            double totalDriveSize = driveInfo.TotalSize;
            double usedDriveSize = driveInfo.TotalSize - driveInfo.AvailableFreeSpace;
            
            response.Stats.Add(new StatModel
            {
                Name = stats[1],
                CpuUsage = new Stat
                {
                    Percentage = ParseSize(stats[2])
                },
                MemoryUsage = new Stat
                {
                    Total = ParseSize(stats[5]),
                    Used = ParseSize(stats[3]),
                    Percentage = ParseSize(stats[6])
                },
                DiskUsage = new Stat
                {
                    Percentage = Math.Round(usedDriveSize / totalDriveSize, 2) * 100,
                    Total = driveInfo.TotalSize,
                    Used = driveInfo.TotalSize - driveInfo.AvailableFreeSpace
                }
            });
        }
            
        _cacheService.SetStats(response);

        return response;
    }

    private static double ParseSize(string toRemove)
    {
        if (toRemove.Contains("%"))
            return Math.Round(double.Parse(toRemove.Replace("%", "")), 2, MidpointRounding.ToZero);

        if (toRemove.Contains("GiB"))
            return Math.Round(double.Parse(toRemove.Replace("GiB", "")) * 1024 * 1048576d, 2,
                MidpointRounding.ToZero);

        if (toRemove.Contains("MiB"))
            return Math.Round(double.Parse(toRemove.Replace("MiB", "")) * 1048576d, 2, MidpointRounding.ToZero);

        return double.Parse(toRemove);
    }
}