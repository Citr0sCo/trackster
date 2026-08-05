import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {catchError, map, Observable, of} from 'rxjs';
import { environment } from '../../environments/environment';
import { IShow } from "./types/show.type";
import { IEpisode } from "./types/episode.type";
import { ISeason } from "./types/season.type";
import { IWatchedEpisode } from "./types/watched-episode.type";
import {ShowMapper} from "./show.mapper";
import {MediaDetailsMapper} from "../media-service/media-details.mapper";
import {IMediaDetails} from "../media-service/types/media-details.type";

@Injectable()
export class ShowRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getAllShowsFor(username: string): Observable<Array<IWatchedEpisode>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows?username=${username}`)
            .pipe(
                map((response: any) => {
                    return response.WatchedEpisodes.map((show: any) => {
                        return {
                            episode: ShowMapper.mapEpisode(show.Episode),
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
                    return ShowMapper.map(response.Show);
                })
            );
    }

    public getShowDetailsBySlug(slug: string): Observable<IMediaDetails | null> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${slug}/details`)
            .pipe(
                map((response: any) => MediaDetailsMapper.map(response.Details)),
                catchError(() => of(null))
            );
    }

    public getSeasonById(identifier: string, seasonNumber: number): Observable<ISeason> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}`)
            .pipe(
                map((response: any) => {
                    return ShowMapper.mapSeason(response.Season);
                })
            );
    }

    public getEpisodeById(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}/episodes/${episodeNumber}`)
            .pipe(
                map((response: any) => {
                    return ShowMapper.mapEpisode(response.Episode);
                })
            );
    }

    public updateEpisodeById(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._httpClient.patch(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}/episodes/${episodeNumber}`, {})
            .pipe(
                map((response: any) => {
                    return ShowMapper.mapEpisode(response.Episode);
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