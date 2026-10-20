import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IMedia } from "./types/media.type";
import { IStats } from './types/stats.type';
import { MediaMapper } from './media.mapper';

@Injectable()
export class MediaRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getPosters(): Observable<Array<string>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/posters`)
            .pipe(
                map((response: any) => {
                    return response;
                })
            );
    }

    public getHistoryForUser(username: string, results: number, page: number): Observable<Array<IMedia>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/history?username=${username}&results=${results}&page=${page}`).pipe()
            .pipe(
                map((response: any) => {
                    return MediaMapper.fromApiArray(response.Media);
                })
            );
    }

    public search(query: string, results: number): Observable<Array<IMedia>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/search?query=${encodeURIComponent(query)}&results=${results}`)
            .pipe(
                map((response: any) => {
                    return MediaMapper.fromApiArray(response.Media);
                })
            );
    }

    public getStats(username: string): Observable<IStats> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/stats?username=${username}`).pipe()
            .pipe(
                map((response: any) => {
                    return {
                        totalWatched: response.Total,
                        totalMoviesWatched: response.MoviesWatched,
                        totalEpisodesWatched: response.EpisodesWatched,
                    };
                })
            );
    }

    public getStatsForCalendar(username: string, daysInThePast: number): Observable<Array<any>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/stats/calendar?username=${username}&daysInThePast=${daysInThePast}`).pipe()
            .pipe(
                map((response: any) => {
                    return response.Stats;
                })
            );
    }
}