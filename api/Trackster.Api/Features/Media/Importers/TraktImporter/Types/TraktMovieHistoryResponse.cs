using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TraktImporter.Types;

public class TraktMovieHistoryResponse
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("watched_at")]
    public DateTime WatchedAt { get; set; }

    [JsonProperty("action")]
    public string Action { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("movie")]
    public MovieType Movie { get; set; }

    public class Ids
    {
        [JsonProperty("trakt")]
        public int Trakt { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("imdb")]
        public string Imdb { get; set; }

        [JsonProperty("tmdb")]
        public int Tmdb { get; set; }
    }

    public class MovieType
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("ids")]
        public Ids Ids { get; set; }
    }

}
