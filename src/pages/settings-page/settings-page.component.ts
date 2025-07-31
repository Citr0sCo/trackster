import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';

@Component({
    selector: 'settings-page',
    templateUrl: './settings-page.component.html',
    styleUrls: ['./settings-page.component.scss'],
    standalone: false
})
export class SettingsPageComponent implements OnInit, OnDestroy {

    public isImporting: boolean = false;
    public username: string = 'citr0s';

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;

    constructor(mediaService: MediaService) {
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {

    }

    public importFromTrakt(): void {
        this.isImporting = true;

        this._mediaService.importFromTrakt('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe(() => {
                this.isImporting = false;
            });
    }

    public saveSettings(): void {

    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
