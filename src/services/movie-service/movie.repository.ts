import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IMovie } from "./types/movie.type";
import { IWatchedMovie } from "./types/watched-movie.type";

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
                            movie: {
                                identifier: movie.Movie.Identifier,
                                title: movie.Movie.Title,
                                slug: movie.Movie.Slug,
                                year: movie.Movie.Year,
                                tmdb: movie.Movie.TMDB,
                                posterUrl: movie.Movie.Poster,
                                overview: movie.Movie.Overview,
                            },
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
                    const media = response.Movie;
                    return {
                        identifier: media.Identifier,
                        mediaType: media.MediaType,
                        title: media.Title,
                        slug: media.Slug,
                        parentTitle: media.ParentTitle,
                        grandParentTitle: media.GrandParentTitle,
                        year: media.Year,
                        tmdb: media.TMDB,
                        posterUrl: media.Poster,
                        overview: media.Overview,
                        seasonNumber: media.SeasonNumber,
                        episodeNumber: media.EpisodeNumber,
                    };
                })
            );
    }

    public getMovieWatchHistoryById(username: string, identifier: string): Observable<Array<IWatchedMovie>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/movies/${identifier}/history?username=${username}`)
            .pipe(
                map((response: any) => {
                    const episodes = response.WatchHistory;
                    return episodes.map((episode: any) => {
                        return {
                            watchedAt: episode.WatchedAt
                        };
                    });
                })
            );
    }

    public updateMovieById(slug: string): Observable<IMovie> {
        return this._httpClient.patch(`${environment.apiBaseUrl}/api/movie/${slug}`, {})
            .pipe(
                map((response: any) => {
                    const movie = response.Movie;
                    return {
                        identifier: movie.Identifier,
                        title: movie.Title,
                    } as IMovie;
                })
            );
    }
}