import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IMovie } from "./types/movie.type";
import { IWatchedMovie } from "./types/watched-movie.type";
import {MovieMapper} from "./movie.mapper";

@Injectable()
export class MovieRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getAllMoviesFor(username: string): Observable<Array<IWatchedMovie>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/movies?username=${username}`)
            .pipe(
                map((response: any) => {
                    return response.WatchedMovies.map((movie: any) => {
                        return {
                            movie: MovieMapper.map(movie.Movie),
                            watchedAt: movie.WatchedAt
                        };
                    });
                })
            );
    }

    public getMovieBySlug(slug: string): Observable<IMovie> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/movies/${slug}`)
            .pipe(
                map((response: any) => {
                    return MovieMapper.map(response.Movie);
                })
            );
    }

    public getMovieWatchHistoryById(username: string, identifier: string): Observable<Array<IWatchedMovie>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/movies/${identifier}/history?username=${username}`)
            .pipe(
                map((response: any) => {
                    const movies = response.WatchHistory;
                    return movies.map((episode: any) => {
                        return {
                            watchedAt: episode.WatchedAt
                        };
                    });
                })
            );
    }

    public updateMovieById(slug: string): Observable<IMovie> {
        return this._httpClient.patch(`${environment.apiBaseUrl}/api/movies/${slug}`, {})
            .pipe(
                map((response: any) => {
                    return MovieMapper.map(response.Movie);
                })
            );
    }
}