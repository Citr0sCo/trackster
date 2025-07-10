import { Injectable } from '@angular/core';
import { ILink } from './types/link.type';
import { Observable, of, Subject, tap } from 'rxjs';
import { MediaRepository } from './media.repository';

@Injectable()
export class MediaService {

    private _mediaRepository: MediaRepository;
    private _cachedLinks: Array<ILink> | null = null;

    constructor(mediaRepository: MediaRepository) {
        this._mediaRepository = mediaRepository;
    }

    public importFromTrakt(username: string): Observable<any> {
        return this._mediaRepository.importFromTrakt(username);
    }
}
