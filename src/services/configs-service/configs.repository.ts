import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ConfigsMapper } from './configs.mapper';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';
import { IConfigs } from './types/configs.type';

@Injectable()
export class ConfigsRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getAllConfigs(): Observable<IConfigs> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/configs`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return ConfigsMapper.map(response);
                })
            );
    }
}