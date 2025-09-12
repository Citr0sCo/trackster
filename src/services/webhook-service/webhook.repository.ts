import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IWebhook } from './types/webhook.type';

@Injectable()
export class WebhookRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getWebhook(userReference: string): Observable<IWebhook> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/webhooks/${userReference}`)
            .pipe(
                map((response: any) => {
                    return {
                        identifier: response.Identifier,
                        userIdentifier: response.UserIdentifier,
                        apiKey: response.ApiKey,
                        provider: response.Provider,
                        url: response.Url,
                    };
                })
            );
    }

    public createWebhook(request: any): Observable<IWebhook> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/webhooks/${request.UserIdentifier}`, request)
            .pipe(
                map((response: any) => {
                    return {
                        identifier: response.Identifier,
                        userIdentifier: response.UserIdentifier,
                        apiKey: response.ApiKey,
                        provider: response.Provider,
                        url: response.Url,
                    };
                })
            );
    }
}