import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, of } from 'rxjs';
import { PlexMapper } from './plex.mapper';
import { environment } from '../../environments/environment';
import { IPlexSession } from './types/plex-session.type';

@Injectable()
export class PlexRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getActivity(): Observable<Array<IPlexSession>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/plex/activity`)
            .pipe(
                map((response: any) => {
                    return PlexMapper.mapActivity(response);
                })
            );
    }

}