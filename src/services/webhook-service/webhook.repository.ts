import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';
import { IWebhook } from './types/webhook.type';
import { AuthenticationService } from '../authentication-service/authentication.service';

@Injectable()
export class WebhookRepository {

    private _httpClient: HttpClient;
    private _authService: AuthenticationService;

    constructor(httpClient: HttpClient, authService: AuthenticationService) {
        this._httpClient = httpClient;
        this._authService = authService;
    }

    public getWebhook(userReference: string): Observable<IWebhook> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/webhooks/${userReference}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
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
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
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