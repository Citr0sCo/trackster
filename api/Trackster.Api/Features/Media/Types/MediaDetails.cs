namespace Trackster.Api.Features.Media.Types;

public class MediaDetails
{
    public string Tagline { get; set; } = string.Empty;
    public string Backdrop { get; set; } = string.Empty;
    public string ImdbUrl { get; set; } = string.Empty;
    public string TraktUrl { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public int Runtime { get; set; }
    public double Rating { get; set; }
    public int RatingCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SeasonCount { get; set; }
    public int EpisodeCount { get; set; }
    public List<CastMember> Cast { get; set; } = new();
}

public class CastMember
{
    public string Name { get; set; } = string.Empty;
    public string Character { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;
}
