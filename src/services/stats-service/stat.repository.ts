import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { StatMapper } from './stat.mapper';
import { IStatResponse } from './types/stat.response';

@Injectable()
export class StatRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getAll(): Observable<IStatResponse> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/stats`)
            .pipe(
                map((response: any) => {
                    return StatMapper.map(response);
                })
            );
    }

}