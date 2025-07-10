using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TraktImporter.Types;

public class TraktMovieResponse
{
    [JsonProperty(PropertyName = "collected_at")]
    public DateTime CollectedAt { get; set; }

    [JsonProperty(PropertyName = "updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

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
            public int Trakt { get; set; }
            
            [JsonProperty(PropertyName = "slug")]
            public string Slug { get; set; }
            
            [JsonProperty(PropertyName = "imdb")]
            public string IMDB { get; set; }
            
            [JsonProperty(PropertyName = "tmdb")]
            public string TMDB { get; set; }
        }
    }
}