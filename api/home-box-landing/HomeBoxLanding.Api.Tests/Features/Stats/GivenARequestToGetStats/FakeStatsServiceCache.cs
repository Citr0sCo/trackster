using HomeBoxLanding.Api.Features.Stats;
using HomeBoxLanding.Api.Features.Stats.Types;

namespace HomeBoxLanding.Api.Tests.Features.Stats.GivenARequestToGetStats;

public class FakeStatsServiceCache : IStatsServiceCache
{
    private StatsResponse? _stats = null;

    public StatsResponse? GetStats()
    {
        return _stats;
    }

    public void SetStats(StatsResponse stats)
    {
        _stats = stats;
    }
}