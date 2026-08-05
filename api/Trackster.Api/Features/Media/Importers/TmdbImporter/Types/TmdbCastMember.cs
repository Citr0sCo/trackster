using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

public class TmdbCastMember
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("character")]
    public string Character { get; set; } = string.Empty;

    [JsonProperty("profile_path")]
    public string? ProfilePath { get; set; }

    [JsonProperty("order")]
    public int Order { get; set; }
}
