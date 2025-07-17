import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';
import { Provider } from "../../core/providers.enum";
import { ImportType } from "../../core/import-type.enum";
import { IMovie } from "./types/movie.type";
import {IShow} from "./types/show.type";
import {IMedia} from "./types/media.type";

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
                            tmdb: movie.TMDB,
                            posterUrl: movie.Poster,
                            overview: movie.Overview,
                            watchedAt: movie.WatchedAt
                        };
                    });
                })
            );
    }

    public getAllShowsFor(username: string): Observable<Array<IShow>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/shows?username=${username}`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return response.Shows.map((movie: any) => {
                        return {
                            identifier: movie.Identifier,
                            title: movie.Title,
                            mediaType: movie.MediaType,
                            parentTitle: movie.ParentTitle,
                            grandParentTitle: movie.GrandParentTitle,
                            year: movie.Year,
                            tmdb: movie.TMDB,
                            posterUrl: movie.Poster,
                            overview: movie.Overview,
                            watchedAt: movie.WatchedAt
                        };
                    });
                })
            );
    }

    public getHistoryForUser(username: string): Observable<Array<IMedia>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/history?username=${username}`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return response.Media.map((media: any) => {
                        return {
                            identifier: media.Identifier,
                            mediaType: media.MediaType,
                            title: media.Title,
                            parentTitle: media.ParentTitle,
                            grandParentTitle: media.GrandParentTitle,
                            year: media.Year,
                            tmdb: media.TMDB,
                            posterUrl: media.Poster,
                            overview: media.Overview,
                            watchedAt: media.WatchedAt
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