import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';

@Component({
    selector: 'home-page',
    templateUrl: './home-page.component.html',
    styleUrls: ['./home-page.component.scss'],
    standalone: false
})
export class HomePageComponent implements OnInit, OnDestroy {

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;

    constructor(mediaService: MediaService) {
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {

    }

    public importFromTrakt(): void {
        this._mediaService.importFromTrakt('citr0s')
            .subscribe();
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
