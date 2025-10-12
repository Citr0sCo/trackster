using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

public class TmdbCommonResponse
{
    [JsonProperty("success")]
    public bool? IsSuccess { get; set; }

    [JsonProperty("status_code")]
    public int? StatusCode { get; set; }

    [JsonProperty("status_message")]
    public string? StatusMessage { get; set; }
}