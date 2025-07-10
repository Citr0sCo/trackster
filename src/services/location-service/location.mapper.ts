import { ILocationData } from './types/location-data.type';

export class LocationMapper {

    public static map(position: any): ILocationData {

        if (position === null) {
            return {
                latitude: 0,
                longitude: 0,
                timestamp: new Date()
            };
        }

        if (position.coords) {
            return {
                latitude: position.coords.latitude,
                longitude: position.coords.longitude,
                timestamp: new Date()
            };
        }

        return {
            latitude: position.latitude,
            longitude: position.longitude,
            timestamp: new Date()
        };
    }

}