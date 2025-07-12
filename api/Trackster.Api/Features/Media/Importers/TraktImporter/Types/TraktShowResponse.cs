using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TraktImporter.Types;

public class TraktShowResponse
{
    [JsonProperty(PropertyName = "last_watched_at")]
    public DateTime LastWatchedAt { get; set; }

    [JsonProperty(PropertyName = "last_updated_at")]
    public DateTime LastUpdatedAt { get; set; }

    [JsonProperty(PropertyName = "plays")]
    public string NumberOfPlays { get; set; }
    
    [JsonProperty(PropertyName = "show")]
    public TraktShow Show { get; set; }

    public class TraktShow
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "year")]
        public int Year { get; set; }
        
        [JsonProperty(PropertyName = "ids")]
        public TraktShowIds Ids { get; set; }

        public class TraktShowIds
        {
            [JsonProperty(PropertyName = "trakt")]
            public string Trakt { get; set; }
            
            [JsonProperty(PropertyName = "slug")]
            public string Slug { get; set; }
            
            [JsonProperty(PropertyName = "tvdb")]
            public string TVDB { get; set; }
            
            [JsonProperty(PropertyName = "imdb")]
            public string IMDB { get; set; }
            
            [JsonProperty(PropertyName = "tmdb")]
            public string TMDB { get; set; }
            
            [JsonProperty(PropertyName = "tvrage")]
            public string TvRage { get; set; }
        }
    }
    
    
    [JsonProperty(PropertyName = "seasons")]
    public List<TraktSeason> Seasons { get; set; }

    public class TraktSeason
    {
        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }
        
        [JsonProperty(PropertyName = "episodes")]
        public List<TraktSeasonEpisode> Episodes { get; set; }

        public class TraktSeasonEpisode
        {
            [JsonProperty(PropertyName = "number")]
            public int Number { get; set; }
            
            [JsonProperty(PropertyName = "last_watched_at")]
            public DateTime WatchedAt { get; set; }

            [JsonProperty(PropertyName = "plays")]
            public string NumberOfPlays { get; set; }
        }
    }
}