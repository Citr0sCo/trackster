import { Injectable } from '@angular/core';
import { Observable, of, tap, } from 'rxjs';
import { MediaRepository } from './media.repository';
import { IMedia } from "./types/media.type";
import { IStats } from './types/stats.type';

@Injectable()
export class MediaService {

    private _mediaRepository: MediaRepository;

    private _cachedHistory: Array<IMedia> = [];
    private _cachedStats: IStats | null = null;
    private _cachedStatsForCalendar: Array<any> = [];
    private _cachedPosters: Array<string> = [];

    constructor(mediaRepository: MediaRepository) {
        this._mediaRepository = mediaRepository;
    }

    public getPosters(): Observable<Array<string>> {

        if (this._cachedPosters.length > 0) {
            return of(this._cachedPosters);
        }

        return this._mediaRepository.getPosters()
            .pipe(
                tap((posters: Array<string>) => {
                    this._cachedPosters = posters;
                })
            );
    }

    public getHistoryForUser(username: string, results: number = 50, page: number = 1): Observable<Array<IMedia>> {

        if (this._cachedHistory.length > 0) {
            return of(this._cachedHistory);
        }

        return this._mediaRepository.getHistoryForUser(username, results, page)
            .pipe(
                tap((media) => {
                    this._cachedHistory = media;
                })
            );
    }

    public getStats(username: string): Observable<IStats> {

        if (this._cachedStats) {
            return of(this._cachedStats);
        }

        return this._mediaRepository.getStats(username)
            .pipe(
                tap((stats) => {
                    this._cachedStats = stats;
                })
            );
    }

    public getStatsForCalendar(username: string, daysInThePast: number = 50): Observable<Array<any>> {

        if (this._cachedStatsForCalendar.length > 0) {
            return of(this._cachedStatsForCalendar);
        }

        return this._mediaRepository.getStatsForCalendar(username, daysInThePast)
            .pipe(
                tap((media) => {
                    this._cachedStatsForCalendar = media;
                })
            );
    }

    public bustCache(): void {
        this._cachedHistory = [];
        this._cachedStats = null;
        this._cachedStatsForCalendar = [];
    }
}
