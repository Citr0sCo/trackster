import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable()
export class BuildRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public updateAllDockerApps(): Observable<string> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/builds/docker-apps`, {})
            .pipe(
                map((response: any) => {
                    return response;
                })
            );
    }

}