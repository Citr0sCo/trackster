import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';
import { IUser } from './types/user.type';
import { AuthenticationService } from '../authentication-service/authentication.service';

@Injectable()
export class UserRepository {

    private _httpClient: HttpClient;
    private _authService: AuthenticationService;

    constructor(httpClient: HttpClient, authService: AuthenticationService) {
        this._httpClient = httpClient;
        this._authService = authService;
    }

    public getUserBySession(): Observable<IUser> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/sessions`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return {
                        identifier: response.User.Identifier,
                        username: response.User.Username,
                        email: response.User.Email,
                        createdAt: response.User.CreatedAt
                    };
                })
            );
    }

    public getUser(userReference: string): Observable<IUser> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/users/${userReference}`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return {
                        identifier: response.User.Identifier,
                        username: response.User.Username,
                        email: response.User.Email,
                        createdAt: response.User.CreatedAt
                    };
                })
            );
    }
}