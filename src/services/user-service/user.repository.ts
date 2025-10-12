import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IUser } from './types/user.type';
import { UserMapper } from './user.mapper';

@Injectable()
export class UserRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getUserBySession(): Observable<IUser> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/sessions`)
            .pipe(
                map((response: any) => {
                    return UserMapper.map(response.User);
                })
            );
    }

    public getUser(userReference: string): Observable<IUser> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/users/${userReference}`)
            .pipe(
                map((response: any) => {
                    return UserMapper.map(response.User);
                })
            );
    }
}