using HomeBoxLanding.Api.Features.Stats.Types;

namespace HomeBoxLanding.Api.Features.Stats;

public interface IStatsServiceCache
{
    StatsResponse? GetStats();
    void SetStats(StatsResponse stats);
}

public class StatsServiceCache : IStatsServiceCache
{
    private static StatsResponse? _cachedStats = null;
    private static StatsServiceCache? _instance = null;

    private StatsServiceCache()
    {
            
    }
        
    public static StatsServiceCache Instance()
    {
        if (_instance == null)
            _instance = new StatsServiceCache();

        return _instance;
    }

    public StatsResponse? GetStats()
    {
        return _cachedStats;
    }

    public void SetStats(StatsResponse stats)
    {
        _cachedStats = stats;
    }
}