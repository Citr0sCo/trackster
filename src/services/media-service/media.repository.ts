import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';

@Injectable()
export class MediaRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public importFromTrakt(username: string): Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/media/import`, { Username: username })
            .pipe(
                mapNetworkError()
            );
    }
}