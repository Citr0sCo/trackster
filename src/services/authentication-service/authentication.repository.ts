import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map, Observable} from 'rxjs';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';

@Injectable()
export class AuthenticationRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public signIn(request: any) : Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/auth/sign-in`, request)
            .pipe(
                mapNetworkError(),
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

    public register(request: any) : Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/auth/register`, request)
            .pipe(
                mapNetworkError(),
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