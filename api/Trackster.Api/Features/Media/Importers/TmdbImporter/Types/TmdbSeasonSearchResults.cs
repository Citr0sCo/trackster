using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

public class TmdbSeasonSearchResults
{
    public TmdbSeasonSearchResults()
    {
        Episodes = new List<Episode>();
    }
    
    [JsonProperty("air_date")]
    public string AirDate { get; set; }

    [JsonProperty("episodes")]
    public List<Episode> Episodes { get; set; }

    [JsonProperty("name")]
    public string Title { get; set; }

    [JsonProperty("overview")]
    public string Overview { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("poster_path")]
    public string PosterPath { get; set; }

    [JsonProperty("season_number")]
    public int SeasonNumber { get; set; }

    [JsonProperty("vote_average")]
    public double VoteAverage { get; set; }
        
    public class Crew
    {
        [JsonProperty("job")]
        public string Job { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("gender")]
        public int Gender { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("known_for_department")]
        public string KnownForDepartment { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("original_name")]
        public string OriginalName { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }
    }

    public class Episode
    {
        [JsonProperty("air_date")]
        public string AirDate { get; set; }

        [JsonProperty("episode_number")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("episode_type")]
        public string EpisodeType { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Title { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("production_code")]
        public string ProductionCode { get; set; }

        [JsonProperty("runtime")]
        public int? Runtime { get; set; }

        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }

        [JsonProperty("show_id")]
        public int ShowId { get; set; }

        [JsonProperty("still_path")]
        public string StillPath { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        [JsonProperty("crew")]
        public List<Crew> Crew { get; set; }

        [JsonProperty("guest_stars")]
        public List<GuestStar> GuestStars { get; set; }
    }

    public class GuestStar
    {
        [JsonProperty("character")]
        public string Character { get; set; }

        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("gender")]
        public int Gender { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("known_for_department")]
        public string KnownForDepartment { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("original_name")]
        public string OriginalName { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }
    }
}