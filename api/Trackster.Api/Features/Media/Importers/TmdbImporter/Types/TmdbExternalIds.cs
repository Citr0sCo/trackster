using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

public class TmdbExternalIds
{
    [JsonProperty("imdb_id")]
    public string? ImdbId { get; set; }
}
