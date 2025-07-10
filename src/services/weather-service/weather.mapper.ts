import { IWeatherData } from './types/weather-data.type';

export class WeatherMapper {

    public static map(response: any): IWeatherData {
        return {
            name: response.name,
            weatherShortDescription: response.weather[0].main,
            weatherDescription: response.weather[0].description,
            latitude: response.coord.lat,
            longitude: response.coord.lon,
            feelsLike: response.main.feels_like,
            humidity: response.main.humidity,
            pressure: response.main.pressure,
            temperature: response.main.temp,
            temperatureMax: response.main.temp_max,
            temperatureMin: response.main.temp_min,
            windDirection: response.wind.deg,
            windGust: response.wind.gust,
            windSpeed: response.wind.speed,
            timestamp: new Date()
        };
    }

}