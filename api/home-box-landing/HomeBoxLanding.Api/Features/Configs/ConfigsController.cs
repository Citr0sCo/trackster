using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.Configs;

[ApiController]
[Route("api/configs")]
public class ConfigsController : ControllerBase
{
    [HttpGet]
    public GetAllConfigsResponse GetAll()
    {
        return new GetAllConfigsResponse
        {
            WeatherApiKey = Environment.GetEnvironmentVariable("ASPNETCORE_WEATHER_API_KEY"),
            MapsApiKey = Environment.GetEnvironmentVariable("ASPNETCORE_MAPS_API_KEY")
        };
    }

    public class GetAllConfigsResponse
    {
        public string? WeatherApiKey { get; set; }
        public string? MapsApiKey { get; set; }
    }
}