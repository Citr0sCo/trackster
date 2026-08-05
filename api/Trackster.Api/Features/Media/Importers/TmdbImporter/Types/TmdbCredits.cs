using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

public class TmdbCredits
{
    [JsonProperty("cast")]
    public List<TmdbCastMember> Cast { get; set; } = new();
}
