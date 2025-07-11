using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TraktImporter.Types;

public class TraktMovieResponse
{
    [JsonProperty(PropertyName = "last_watched_at")]
    public DateTime LastWatchedAt { get; set; }

    [JsonProperty(PropertyName = "last_updated_at")]
    public DateTime LastUpdatedAt { get; set; }

    [JsonProperty(PropertyName = "plays")]
    public string NumberOfPlays { get; set; }

    [JsonProperty(PropertyName = "movie")]
    public TraktMovie Movie { get; set; }

    public class TraktMovie
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "year")]
        public int Year { get; set; }
        
        [JsonProperty(PropertyName = "ids")]
        public TraktMovieIds Ids { get; set; }

        public class TraktMovieIds
        {
            [JsonProperty(PropertyName = "trakt")]
            public string Trakt { get; set; }
            
            [JsonProperty(PropertyName = "slug")]
            public string Slug { get; set; }
            
            [JsonProperty(PropertyName = "imdb")]
            public string IMDB { get; set; }
            
            [JsonProperty(PropertyName = "tmdb")]
            public string TMDB { get; set; }
        }
    }
}