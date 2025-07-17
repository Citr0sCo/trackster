import { Injectable } from '@angular/core';
import {IMovie} from './types/movie.type';
import { Observable, } from 'rxjs';
import { MediaRepository } from './media.repository';
import { IShow } from './types/show.type';
import {IMedia} from "./types/media.type";

@Injectable()
export class MediaService {

    private _mediaRepository: MediaRepository;

    constructor(mediaRepository: MediaRepository) {
        this._mediaRepository = mediaRepository;
    }

    public getAllMoviesFor(username: string): Observable<Array<IMovie>> {
        return this._mediaRepository.getAllMoviesFor(username);
    }

    public getAllShowsFor(username: string): Observable<Array<IShow>> {
        return this._mediaRepository.getAllShowsFor(username);
    }

    public getHistoryForUser(username: string): Observable<Array<IMedia>> {
        return this._mediaRepository.getHistoryForUser(username);
    }

    public importFromTrakt(username: string): Observable<any> {
        return this._mediaRepository.importFromTrakt(username);
    }
}
