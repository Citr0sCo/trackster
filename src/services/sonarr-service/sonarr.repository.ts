import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { SonarrMapper } from './sonarr.mapper';
import { environment } from '../../environments/environment';
import { ISonarrActivity } from './types/sonarr-activity.type';

@Injectable()
export class SonarrRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getActivity(): Observable<ISonarrActivity> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/sonarr/activity`)
            .pipe(
                map((response: any) => {
                    return SonarrMapper.mapActivity(response);
                })
            );
    }

}