import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ISettings } from './types/settings.type';

@Injectable()
export class SettingsRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getSettings(): Observable<ISettings> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/settings`)
            .pipe(
                map((response: any) => {
                    return {
                        traktClientId: response.Settings.TraktClientId,
                    };
                })
            );
    }
}