using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.PiHole.Types;

public class PiHoleActivityResponse
{
    public Guid? Identifier { get; set; }
    
    [JsonProperty("queries")]
    public PiHoleActivityQueries Queries { get; set; }
    
    [JsonProperty("clients")]
    public PiHoleActivityClients Clients { get; set; }

    public class PiHoleActivityQueries
    {
        [JsonProperty("total")]
        public int Total { get; set; }
        
        [JsonProperty("percent_blocked")]
        public double PercentBlocked { get; set; }

        [JsonProperty("blocked")]
        public int Blocked { get; set; }
    }

    public class PiHoleActivityClients
    {
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}