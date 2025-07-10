import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { PiHoleMapper } from './piHoleMapper';
import { environment } from '../../environments/environment';
import { IPiHoleActivity } from './types/pihole-activity.type';

@Injectable()
export class PiHoleRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getActivity(identifier: string): Observable<IPiHoleActivity> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/pihole/activity?identifier=${identifier}`)
            .pipe(
                map((response: any) => {
                    return PiHoleMapper.mapActivity(response);
                })
            );
    }

}