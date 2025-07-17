import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import {IMedia} from "../../services/media-service/types/media.type";

@Component({
    selector: 'history-page',
    templateUrl: './history-page.component.html',
    styleUrls: ['./history-page.component.scss'],
    standalone: false
})
export class HistoryPageComponent implements OnInit, OnDestroy {

    public media: Array<IMedia> = [];

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;

    constructor(mediaService: MediaService) {
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this._mediaService.getHistoryForUser('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe((media) => {
                this.media = media;
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
