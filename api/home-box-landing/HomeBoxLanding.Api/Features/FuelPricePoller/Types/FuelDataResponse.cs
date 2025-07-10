using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.FuelPricePoller.Types;

public class FuelDataResponse
{
    public FuelDataResponse()
    {
        Stations = new List<TescoFuelDataStation>();
    }
    
    [JsonProperty("last_updated")]
    public string? LastUpdated { get; set; }
    
    [JsonProperty("stations")]
    public List<TescoFuelDataStation> Stations { get; set; }
}

public class TescoFuelDataStation
{
    [JsonProperty("site_id")]
    public string? SiteId { get; set; }
    
    [JsonProperty("brand")]
    public string? Brand { get; set; }
    
    [JsonProperty("address")]
    public string? Address { get; set; }
    
    [JsonProperty("postcode")]
    public string? Postcode { get; set; }
    
    [JsonProperty("location")]
    public TescoFuelLocation? Location { get; set; }
    
    [JsonProperty("prices")]
    public TescoFuelPrices? Prices { get; set; }
}

public class TescoFuelLocation
{
    [JsonProperty("latitude")]
    public double? Latitude { get; set; }
    
    [JsonProperty("longitude")]
    public double? Longitude { get; set; }
}

public class TescoFuelPrices
{
    [JsonProperty("E5")]
    public double? PetrolE5 { get; set; }
    
    [JsonProperty("E10")]
    public double? PetrolE10 { get; set; }
    
    [JsonProperty("B7")]
    public double? DieselB7 { get; set; }
}