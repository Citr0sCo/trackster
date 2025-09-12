import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';
import { AuthenticationService } from './authentication.service';

@Injectable()
export class AuthenticationRepository {

    private _httpClient: HttpClient;
    private _authService: AuthenticationService;

    constructor(httpClient: HttpClient, authService: AuthenticationService) {
        this._httpClient = httpClient;
        this._authService = authService;
    }

    public signIn(request: any): Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/auth/sign-in`, request)
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
                        sessionId: response.SessionId,
                        hasError: response.HasError,
                        error: {
                            code: response.Error?.Code,
                            userMessage: response.Error?.UserMessage,
                            technicalMessage: response.Error?.TechnicalMessage
                        }
                    };
                })
            );
    }

    public register(request: any): Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/auth/register`, request)
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
                        sessionId: response.SessionId,
                        hasError: response.HasError,
                        error: {
                            code: response.Error?.Code,
                            userMessage: response.Error?.UserMessage,
                            technicalMessage: response.Error?.TechnicalMessage
                        }
                    };
                })
            );
    }

    public signOut(identifier: string): Observable<any> {
        return this._httpClient.delete(`${environment.apiBaseUrl}/api/auth/sign-out/${identifier}`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return {
                        hasError: response.HasError,
                        error: {
                            code: response.Error?.Code,
                            userMessage: response.Error?.UserMessage,
                            technicalMessage: response.Error?.TechnicalMessage
                        }
                    };
                })
            );
    }
}