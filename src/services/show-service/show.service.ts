import { Injectable } from '@angular/core';
import { Observable, of, tap, } from 'rxjs';
import { ShowRepository } from './show.repository';
import { IShow } from './types/show.type';
import { ISeason } from "./types/season.type";
import { IEpisode } from "./types/episode.type";
import { IWatchedEpisode } from "./types/watched-episode.type";

@Injectable()
export class ShowService {

    private _repository: ShowRepository;

    private _cachedAllShows: Array<IWatchedEpisode> = [];

    constructor(repository: ShowRepository) {
        this._repository = repository;
    }

    public getAllShowsFor(username: string): Observable<Array<IWatchedEpisode>> {

        if (this._cachedAllShows.length > 0) {
            return of(this._cachedAllShows);
        }

        return this._repository.getAllShowsFor(username)
            .pipe(
                tap((shows) => {
                    this._cachedAllShows = shows;
                })
            );
    }

    public getShowBySlug(slug: string): Observable<IShow> {
        return this._repository.getShowBySlug(slug);
    }

    public getSeasonByNumber(identifier: string, seasonNumber: number): Observable<ISeason> {
        return this._repository.getSeasonById(identifier, seasonNumber);
    }

    public getEpisodeByNumber(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._repository.getEpisodeById(identifier, seasonNumber, episodeNumber);
    }

    public updateEpisodeById(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._repository.updateEpisodeById(identifier, seasonNumber, episodeNumber);
    }

    public getEpisodeWatchHistory(username: string, identifier: string, seasonNumber: number, episodeNumber: number): Observable<Array<IWatchedEpisode>> {
        return this._repository.getEpisodeWatchHistory(username, identifier, seasonNumber, episodeNumber);
    }

    public bustCache(): void {
        this._cachedAllShows = [];
    }
}
