namespace HomeBoxLanding.Api.Features.FuelPricePoller.Types;

public class GetAroundLocationResponse
{
    public GetAroundLocationResponse()
    {
        Stations = new List<FuelPriceModel>();
    }
    
    public List<FuelPriceModel> Stations { get; set; }
}