using HomeBoxLanding.Api.Features.FuelPricePoller.Types;
using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.FuelPricePoller;

[ApiController]
[Route("api/fuel-prices")]
public class FuelPricePollerController : ControllerBase
{
    private readonly FuelPriceService _service;

    public FuelPricePollerController()
    {
        _service = new FuelPriceService(new FuelPriceRepository());
    }

    [HttpGet]
    public GetAroundLocationResponse GetAroundLocation([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] int range, [FromQuery] int? maxResults)
    {
        return new GetAroundLocationResponse
        {
            Stations = _service.GetClosestTo(latitude, longitude, range, maxResults ?? 1000)
        };
    }
}