import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IShow } from "./types/show.type";
import { IWatchedShow } from "./types/watched-show.type";
import { IEpisode } from "./types/episode.type";
import { ISeason } from "./types/season.type";
import { IWatchedEpisode } from "./types/watched-episode.type";

@Injectable()
export class ShowRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getAllShowsFor(username: string): Observable<Array<IWatchedShow>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows?username=${username}`)
            .pipe(
                map((response: any) => {
                    return response.WatchedShows.map((show: any) => {
                        return {
                            show: {
                                identifier: show.Show.Identifier,
                                title: show.Show.Title,
                                slug: show.Show.Slug,
                                year: show.Show.Year,
                                tmdb: show.Show.TMDB,
                                posterUrl: show.Show.Poster,
                                overview: show.Show.Overview,
                            },
                            season: {
                                identifier: show.Season.Identifier,
                                title: show.Season.Title,
                                number: show.Season.Number
                            },
                            episode: {
                                identifier: show.Episode.Identifier,
                                title: show.Episode.Title,
                                number: show.Episode.Number
                            },
                            watchedAt: show.WatchedAt
                        };
                    });
                })
            );
    }

    public getShowBySlug(slug: string): Observable<IShow> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${slug}`)
            .pipe(
                map((response: any) => {
                    const show = response.Show;
                    return {
                        identifier: show.Identifier,
                        title: show.Title,
                        slug: show.Slug,
                        year: show.Year,
                        tmdb: show.TMDB,
                        posterUrl: show.Poster,
                        overview: show.Overview,
                    };
                })
            );
    }

    public getSeasonById(identifier: string, seasonNumber: number): Observable<ISeason> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}`)
            .pipe(
                map((response: any) => {
                    const season = response.Season;
                    return {
                        identifier: season.Identifier,
                        title: season.Title,
                        number: season.Number
                    };
                })
            );
    }

    public getEpisodeById(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}/episodes/${episodeNumber}`)
            .pipe(
                map((response: any) => {
                    const episode = response.Episode;
                    return {
                        identifier: episode.Identifier,
                        title: episode.Title,
                        number: episode.Number
                    };
                })
            );
    }

    public updateEpisodeById(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._httpClient.patch(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}/episodes/${episodeNumber}`, {})
            .pipe(
                map((response: any) => {
                    const episode = response.Episode;
                    return {
                        identifier: episode.Identifier,
                        title: episode.Title,
                        number: episode.Number
                    };
                })
            );
    }

    public getEpisodeWatchHistory(username: string, identifier: string, seasonNumber: number, episodeNumber: number): Observable<Array<IWatchedEpisode>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}/episodes/${episodeNumber}/history?username=${username}`)
            .pipe(
                map((response: any) => {
                    const episodes = response.WatchedEpisodes;
                    return episodes.map((episode: any) => {
                        return {
                            watchedAt: episode.WatchedAt
                        };
                    });
                })
            );
    }
}