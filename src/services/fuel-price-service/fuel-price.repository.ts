import { Injectable, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, Subscription } from 'rxjs';
import { FuelPriceMapper } from './fuel-price.mapper';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';
import { LocationService } from '../location-service/location.service';
import { IFuelPrice } from './types/fuel-price.type';

@Injectable()
export class FuelPriceRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getAroundLocation(latitude: number | null, longitude: number | null, locationRange: string): Observable<Array<IFuelPrice>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/fuel-prices?latitude=${latitude}&longitude=${longitude}&range=${locationRange}`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return FuelPriceMapper.map(response.Stations);
                })
            );
    }
}