namespace Trackster.Api.Features.Media.Types;

public class GetStatsForCalendarResonse
{
    public Dictionary<string, int> Stats { get; set; }
}

public class GetStatsResponse
{
    public int Total { get; set; }
    public int MoviesWatched { get; set; }
    public int EpisodesWatched { get; set; }
}