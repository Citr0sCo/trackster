export interface IWeatherData {
    name: string;
    weatherShortDescription: string;
    weatherDescription: string;
    latitude: number;
    longitude: number;
    feelsLike: number;
    humidity: number;
    pressure: number;
    temperature: number;
    temperatureMax: number;
    temperatureMin: number;
    windDirection: number;
    windGust: number;
    windSpeed: number;
    timestamp: Date;
}