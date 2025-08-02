using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TraktImporter.Types;

public class TraktEpisodeHistoryResponse
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("watched_at")]
    public DateTime WatchedAt { get; set; }

    [JsonProperty("action")]
    public string Action { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("episode")]
    public EpisodeType Episode { get; set; }

    [JsonProperty("show")]
    public ShowType Show { get; set; }

    public class EpisodeType
    {
        [JsonProperty("season")]
        public int Season { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("ids")]
        public Ids Ids { get; set; }
    }

    public class Ids
    {
        [JsonProperty("trakt")]
        public int Trakt { get; set; }

        [JsonProperty("tvdb")]
        public int Tvdb { get; set; }

        [JsonProperty("imdb")]
        public string Imdb { get; set; }

        [JsonProperty("tmdb")]
        public int Tmdb { get; set; }

        [JsonProperty("tvrage")]
        public object Tvrage { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }
    }

    public class ShowType
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("ids")]
        public Ids Ids { get; set; }
    }
}

