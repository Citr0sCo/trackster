using System.Globalization;
using HomeBoxLanding.Api.Data;
using HomeBoxLanding.Api.Features.FuelPricePoller.Types;
using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.FuelPricePoller;

public class FuelPriceRepository
{
    public List<FuelPriceModel> GetFuelPrices(double latitude, double longitude, int rangeInMeters = 10000, int maxResults = 1000)
    {
        using (var context = new DatabaseContext())
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                var records = context.FuelPrices.ToList();
                return records
                    .Select(x => new FuelPriceModel
                    {
                        Identifier = x.Identifier,
                        Name = x.Name,
                        Address = x.Address,
                        Postcode = x.Postcode,
                        Provider = x.Provider,
                        Brand = x.Brand,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        Petrol_E5_Price = x.Petrol_E5_Price ?? 0,
                        Petrol_E10_Price = x.Petrol_E10_Price ?? 0,
                        Diesel_B7_Price = x.Diesel_B7_Price ?? 0,
                        UpdatedAt = x.UpdatedAt,
                        CreatedAt = x.CreatedAt,
                        DistanceInMeters = Haversine.Calculate(latitude, longitude, x.Latitude, x.Longitude)
                    })
                    .Where(x => x.DistanceInMeters < rangeInMeters)
                    .OrderBy(x => x.Petrol_E10_Price)
                    .ThenBy(x => x.DistanceInMeters)
                    .Take(maxResults)
                    .ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return new List<FuelPriceModel>();
        }
    }
    
    public async Task SaveFuelPricesFor(FuelProvider provider, string data)
    {
        var records = ParseDataBasedOnProvider(provider, data);

        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (var record in records)
                {
                    var existingRecord = context.FuelPrices.FirstOrDefault(x => x.Name == record.Name);
                    
                    if(existingRecord == null)
                        await context.AddAsync(record).ConfigureAwait(false);
                    else
                    {
                        existingRecord.Address = record.Address;
                        existingRecord.Postcode = record.Postcode;
                        existingRecord.Provider = provider;
                        existingRecord.Brand = record.Brand;
                        existingRecord.Latitude = record.Latitude;
                        existingRecord.Longitude = record.Longitude;
                        existingRecord.Petrol_E5_Price = record.Petrol_E5_Price;
                        existingRecord.Petrol_E10_Price = record.Petrol_E10_Price;
                        existingRecord.Diesel_B7_Price = record.Diesel_B7_Price;
                        existingRecord.UpdatedAt = record.UpdatedAt;
                    }
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                Console.WriteLine(exception);
            }
        }
    }

    private List<FuelPriceRecord> ParseDataBasedOnProvider(FuelProvider provider, string data)
    {
        var records = new List<FuelPriceRecord>();
        
        var parsedData = JsonConvert.DeserializeObject<FuelDataResponse>(data);
        
        foreach (var station in parsedData?.Stations ?? new List<TescoFuelDataStation>())
        {
            var parsedDate = DateTime.ParseExact(parsedData.LastUpdated, "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            
            records.Add(new FuelPriceRecord
            {
                Identifier = Guid.NewGuid(),
                Name = station.SiteId,
                Address = station.Address,
                Postcode = station.Postcode,
                Provider = provider,
                Brand = station.Brand,
                Latitude = station?.Location?.Latitude ?? 0,
                Longitude = station?.Location?.Longitude ?? 0,
                Petrol_E5_Price = Math.Round((station?.Prices?.PetrolE5 > 100 ? station.Prices.PetrolE5 / 100 : station?.Prices?.PetrolE5) ?? 0, 3),
                Petrol_E10_Price = Math.Round((station?.Prices?.PetrolE10 > 100 ? station.Prices.PetrolE10 / 100 : station?.Prices?.PetrolE10) ?? 0, 3),
                Diesel_B7_Price = Math.Round((station?.Prices?.DieselB7 > 100 ? station.Prices.DieselB7 / 100 : station?.Prices?.DieselB7) ?? 0, 3),
                UpdatedAt = new DateTime(parsedDate.Ticks, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            });
        }

        return records;
    }
}