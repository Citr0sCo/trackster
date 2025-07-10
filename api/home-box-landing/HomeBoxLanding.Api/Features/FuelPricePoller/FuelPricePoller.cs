using HomeBoxLanding.Api.Core.Events.Types;

namespace HomeBoxLanding.Api.Features.FuelPricePoller;

public class FuelPricePoller : ISubscriber
{
    private static FuelPricePoller _instance;
    private bool _isPolling = false;
    
    private Dictionary<FuelProvider, string> _fuelProviders = new Dictionary<FuelProvider, string>
    {
        { FuelProvider.Tesco, "https://www.tesco.com/fuel_prices/fuel_prices_data.json" },
        { FuelProvider.Asda, "https://storelocator.asda.com/fuel_prices_data.json" },
        { FuelProvider.Applegreen, "https://applegreenstores.com/fuel-prices/data.json" },
        { FuelProvider.Ascona, "https://fuelprices.asconagroup.co.uk/newfuel.json" },
        { FuelProvider.BP, "https://www.bp.com/en_gb/united-kingdom/home/fuelprices/fuel_prices_data.json" },
        { FuelProvider.EssoTescoAlliance, "https://www.esso.co.uk/-/media/Project/WEP/Esso/Esso-Retail-UK/roadfuelpricingscheme " },
        { FuelProvider.Morrisons, "https://www.morrisons.com/fuel-prices/fuel.json" },
        { FuelProvider.MotorFuelGroup, "https://fuel.motorfuelgroup.com/fuel_prices_data.json" },
        { FuelProvider.Rontec, "https://www.rontec-servicestations.co.uk/fuel-prices/data/fuel_prices_data.json" },
        { FuelProvider.Sainsburys, "https://api.sainsburys.co.uk/v1/exports/latest/fuel_prices_data.json" },
        { FuelProvider.SGN, "https://www.sgnretail.uk/files/data/SGN_daily_fuel_prices.json" },
        { FuelProvider.Shell, "https://www.shell.co.uk/fuel-prices-data.html" }
    };

    private readonly FuelPriceRepository _repository;

    public FuelPricePoller()
    {
        _repository = new FuelPriceRepository();
    }

    public static ISubscriber Instance()
    {
        if (_instance == null)
            _instance = new FuelPricePoller();

        return _instance;
    }

    private async Task StartPolling()
    {
        while (_isPolling)
        {
            Console.WriteLine("Grabbing latest data from fuel providers...");

            foreach (var fuelProvider in _fuelProviders)
            {
                try
                {
                    var httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    httpClient.DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    httpClient.DefaultRequestHeaders.Add("Accept-Language","en-GB,en-US;q=0.9,en;q=0.8");
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    
                    var result = await httpClient.GetAsync(fuelProvider.Value).ConfigureAwait(false);
                    var response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                    
                    await _repository.SaveFuelPricesFor(fuelProvider.Key, response).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to grab fuel data for {fuelProvider.Key}.");
                }
            }
            
            Console.WriteLine("Finished grabbing latest data from fuel providers, waiting for 60 minutes...");
            Thread.Sleep(1000 * 60 * 60); // 60 Minutes
        }
    }

    public void OnStarted()
    {
        _isPolling = true;
        StartPolling().ConfigureAwait(false);
    }

    public void OnStopping()
    {
        _isPolling = false;
    }

    public void OnStopped()
    {
        _isPolling = false;
    }
}

public enum FuelProvider
{
    Unknown = 0,
    Tesco,
    Asda,
    Applegreen,
    Ascona,
    BP,
    EssoTescoAlliance,
    Morrisons,
    MotorFuelGroup,
    Rontec,
    Sainsburys,
    SGN,
    Shell
}