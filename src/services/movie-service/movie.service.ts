import { Injectable } from '@angular/core';
import { IMovie } from './types/movie.type';
import { Observable, of, tap, } from 'rxjs';
import { MovieRepository } from './movie.repository';
import { IWatchedMovie } from "./types/watched-movie.type";

@Injectable()
export class MovieService {

    private _repository: MovieRepository;

    private _cachedAllMovies: Array<IWatchedMovie> = [];

    constructor(repository: MovieRepository) {
        this._repository = repository;
    }

    public getAllMoviesFor(username: string): Observable<Array<IWatchedMovie>> {

        if (this._cachedAllMovies.length > 0) {
            return of(this._cachedAllMovies);
        }

        return this._repository.getAllMoviesFor(username)
            .pipe(
                tap((movies) => {
                    this._cachedAllMovies = movies;
                })
            );
    }

    public getMovieBySlug(slug: string): Observable<IMovie> {
        return this._repository.getMovieBySlug(slug);
    }

    public getMovieWatchHistoryById(username: string, identifier: any) {
        return this._repository.getMovieWatchHistoryById(username, identifier);
    }

    public updateMovieById(slug: string): Observable<IMovie> {
        return this._repository.updateMovieById(slug);
    }

    public bustCache(): void {
        this._cachedAllMovies = [];
    }
}
