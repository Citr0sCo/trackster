import { Injectable } from '@angular/core';
import {IMovie} from './types/movie.type';
import { Observable, } from 'rxjs';
import { MediaRepository } from './media.repository';

@Injectable()
export class MediaService {

    private _mediaRepository: MediaRepository;

    constructor(mediaRepository: MediaRepository) {
        this._mediaRepository = mediaRepository;
    }

    public getAllMoviesFor(username: string): Observable<Array<IMovie>> {
        return this._mediaRepository.getAllMoviesFor(username);
    }

    public importFromTrakt(username: string): Observable<any> {
        return this._mediaRepository.importFromTrakt(username);
    }
}
