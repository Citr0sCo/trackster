import { Injectable } from '@angular/core';
import { IMovie } from './types/movie.type';
import { Observable, of, tap, } from 'rxjs';
import { MediaRepository } from './media.repository';
import { IShow } from './types/show.type';
import { IMedia } from "./types/media.type";
import { IWatchedMovie } from "./types/watched-movie.type";
import { IWatchedShow } from "./types/watched-show.type";
import { ISeason } from "./types/season.type";
import { IEpisode } from "./types/episode.type";
import { IWatchedEpisode } from "./types/watched-episode.type";

@Injectable()
export class MediaService {

    private _mediaRepository: MediaRepository;

    private _cachedAllMovies: Array<IWatchedMovie> = [];
    private _cachedAllShows: Array<IWatchedShow> = [];
    private _cachedHistory: Array<IMedia> = [];

    constructor(mediaRepository: MediaRepository) {
        this._mediaRepository = mediaRepository;
    }

    public getAllMoviesFor(username: string): Observable<Array<IWatchedMovie>> {

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

    public getAllShowsFor(username: string): Observable<Array<IWatchedShow>> {

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

    public getHistoryForUser(username: string, results: number = 50, page: number = 1): Observable<Array<IMedia>> {

        if (this._cachedHistory.length > 0) {
            return of(this._cachedHistory.slice(0, results));
        }

        return this._mediaRepository.getHistoryForUser(username, results, page)
            .pipe(
                tap((media) => {
                    this._cachedHistory = media;
                })
            );
    }

    public importFromTrakt(username: string, debug: boolean): Observable<any> {
        return this._mediaRepository.importFromTrakt(username, debug);
    }

    public getMovieBySlug(slug: string): Observable<IMovie> {
        return this._mediaRepository.getMovieBySlug(slug);
    }

    public getMovieWatchHistoryById(username: string, identifier: any) {
        return this._mediaRepository.getMovieWatchHistoryById(username, identifier);
    }

    public getShowBySlug(slug: string): Observable<IShow> {
        return this._mediaRepository.getShowBySlug(slug);
    }

    public getSeasonByNumber(identifier: string, seasonNumber: number): Observable<ISeason> {
        return this._mediaRepository.getSeasonById(identifier, seasonNumber);
    }

    public getEpisodeByNumber(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._mediaRepository.getEpisodeById(identifier, seasonNumber, episodeNumber);
    }

    public getEpisodeWatchHistory(username: string, identifier: string, seasonNumber: number, episodeNumber: number): Observable<Array<IWatchedEpisode>> {
        return this._mediaRepository.getEpisodeWatchHistory(username, identifier, seasonNumber, episodeNumber);
    }

    public bustCache(): void {
        this._cachedAllMovies = [];
        this._cachedAllShows = [];
        this._cachedHistory = [];
    }
}
