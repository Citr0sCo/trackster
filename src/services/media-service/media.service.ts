import { Injectable } from '@angular/core';
import { IMovie } from './types/movie.type';
import { Observable, of, tap, } from 'rxjs';
import { MediaRepository } from './media.repository';
import { IShow } from './types/show.type';
import { IMedia } from "./types/media.type";

@Injectable()
export class MediaService {

    private _mediaRepository: MediaRepository;

    private _cachedAllMovies: Array<IMovie> = [];
    private _cachedAllShows: Array<IShow> = [];
    private _cachedHistory: Array<IMedia> = [];

    constructor(mediaRepository: MediaRepository) {
        this._mediaRepository = mediaRepository;
    }

    public getAllMoviesFor(username: string): Observable<Array<IMovie>> {

        if (this._cachedAllMovies.length > 0) {
            return of(this._cachedAllMovies);
        }

        return this._mediaRepository.getAllMoviesFor(username)
            .pipe(
                tap((movies) => {
                    this._cachedAllMovies = movies;
                })
            );
    }

    public getAllShowsFor(username: string): Observable<Array<IShow>> {

        if (this._cachedAllShows.length > 0) {
            return of(this._cachedAllShows);
        }

        return this._mediaRepository.getAllShowsFor(username)
            .pipe(
                tap((shows) => {
                    this._cachedAllShows = shows;
                })
            );
    }

    public getHistoryForUser(username: string): Observable<Array<IMedia>> {

        if (this._cachedHistory.length > 0) {
            return of(this._cachedHistory);
        }

        return this._mediaRepository.getHistoryForUser(username)
            .pipe(
                tap((media) => {
                    this._cachedHistory = media;
                })
            );
    }

    public importFromTrakt(username: string): Observable<any> {
        return this._mediaRepository.importFromTrakt(username);
    }

    public getMediaById(identifier: string): Observable<IMedia> {
        return this._mediaRepository.getMediaById(identifier);
    }

    public bustCache(): void {
        this._cachedAllMovies = [];
        this._cachedAllShows = [];
        this._cachedHistory = [];
    }
}
