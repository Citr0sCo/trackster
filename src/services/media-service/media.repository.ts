import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map, Observable} from 'rxjs';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';
import {Provider} from "../../core/providers.enum";
import {ImportType} from "../../core/import-type.enum";
import {IMovie} from "./types/movie.type";

@Injectable()
export class MediaRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getAllMoviesFor(username: string): Observable<Array<IMovie>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/movies?username=${username}`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return response.Movies.map((movie: any) => {
                        return {
                            identifier: movie.Identifier,
                            title: movie.Title,
                            year: movie.Year,
                            tmdb: movie.TMDB
                        };
                    });
                })
            );
    }

    public importFromTrakt(username: string): Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/media/import`, { Type: ImportType.Trakt, Username: username })
            .pipe(
                mapNetworkError()
            );
    }
}