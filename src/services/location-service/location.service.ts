import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ILocationData } from './types/location-data.type';
import { LocationMapper } from './location.mapper';

@Injectable()
export class LocationService {

    private _cachedLocation: ILocationData | null = null;

    public getLocation(): Observable<ILocationData> {

        if (localStorage.getItem('cachedLocation')) {
            this._cachedLocation = JSON.parse(`${localStorage.getItem('cachedLocation')}`);
        }

        if (this._cachedLocation !== null) {
            return of(this._cachedLocation);
        }

        return this.getCurrentLocation();
    }

    public getCurrentLocation(force: boolean = false): Observable<ILocationData> {

        if (this._cachedLocation !== null && !force) {

            this._cachedLocation.timestamp = new Date();

            const differenceInTime = new Date().getTime() - new Date(this._cachedLocation.timestamp).getTime();

            const hourInMilliseconds = 3600 * 1000;

            if (differenceInTime < hourInMilliseconds) {
                return of(this._cachedLocation);
            }
        }

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition((position) => {
                localStorage.setItem('cachedLocation', JSON.stringify(LocationMapper.map(position)));
                this._cachedLocation = LocationMapper.map(position);
                return LocationMapper.map(position);
            });
        } else {
            console.error('Geolocation is not supported by this browser.');
        }

        return of(LocationMapper.map(this._cachedLocation));
    }

}