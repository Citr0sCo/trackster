import { IConfigs } from './types/configs.type';

export class ConfigsMapper {

    public static map(response: any): IConfigs {
        return {
            weatherApiKey: response.WeatherApiKey,
            mapsApiKey: response.MapsApiKey
        };
    }
}