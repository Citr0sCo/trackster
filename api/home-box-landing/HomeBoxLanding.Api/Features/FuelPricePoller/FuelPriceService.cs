using HomeBoxLanding.Api.Features.FuelPricePoller.Types;

namespace HomeBoxLanding.Api.Features.FuelPricePoller;

public class FuelPriceService
{
    private readonly FuelPriceRepository _repository;

    public FuelPriceService(FuelPriceRepository repository)
    {
        _repository = repository;
    }

    public List<FuelPriceModel> GetClosestTo(double latitude, double longitude, int rangeInKilometers, int maxResults)
    {
        return _repository.GetFuelPrices(latitude, longitude, rangeInKilometers * 1000, maxResults);
    }
}