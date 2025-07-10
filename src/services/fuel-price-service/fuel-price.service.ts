import { Injectable } from '@angular/core';
import { IFuelPrice } from './types/fuel-price.type';
import { Observable, of, tap } from 'rxjs';
import { FuelPriceRepository } from './fuel-price.repository';
import { ILocationData } from '../location-service/types/location-data.type';

@Injectable()
export class FuelPriceService {

    private _fuelPriceRepository: FuelPriceRepository;
    private _cachedFuelStations: { stations: Array<IFuelPrice>; timestamp: Date } | null = null;

    constructor(fuelPriceRepository: FuelPriceRepository) {
        this._fuelPriceRepository = fuelPriceRepository;
    }

    public refreshCache(locationData: ILocationData, locationRange: string): Observable<Array<IFuelPrice>> {
        return this._fuelPriceRepository.getAroundLocation(locationData.latitude ?? null, locationData.longitude ?? null, locationRange)
            .pipe(
                tap((fuelStations) => {
                    this._cachedFuelStations = { stations: fuelStations, timestamp: new Date() };
                    localStorage.setItem('cachedFuelStations', JSON.stringify(fuelStations));
                })
            );
    }

    public getAroundLocation(locationData: ILocationData, locationRange: string, force: boolean = false): Observable<Array<IFuelPrice>> {

        if (localStorage.getItem('cachedFuelStations')) {
            this._cachedFuelStations = { stations: JSON.parse(`${localStorage.getItem('cachedFuelStations')}`), timestamp: new Date() };
        }

        if (this._cachedFuelStations !== null && this._cachedFuelStations.stations.length > 0 && !force) {
            const differenceInTime = new Date().getTime() - new Date(this._cachedFuelStations.timestamp).getTime();

            const hourInMilliseconds = 1000 * 60 * 60;

            if (differenceInTime < hourInMilliseconds) {
                return of(this._cachedFuelStations.stations);
            }
        }

        return this.refreshCache(locationData, locationRange);
    }
}
