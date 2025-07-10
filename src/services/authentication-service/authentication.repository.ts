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
        return this._httpClient.post(`${environment.apiBaseUrl}/api/authentication/sign-in`, request)
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