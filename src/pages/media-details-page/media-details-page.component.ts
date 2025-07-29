import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {MediaService} from "../../services/media-service/media.service";
import {Subject, takeUntil} from "rxjs";
import {IMedia} from "../../services/media-service/types/media.type";

@Component({
    selector: 'media-details-page',
    templateUrl: './media-details-page.component.html',
    styleUrls: ['./media-details-page.component.scss'],
    standalone: false
})
export class MediaDetailsPageComponent implements OnInit, OnDestroy {

    public media: IMedia | null = null;

    private readonly _destroy: Subject<void> = new Subject();
    private _activatedRoute: ActivatedRoute;
    private _mediaService: MediaService;

    constructor(activatedRoute: ActivatedRoute, mediaService: MediaService) {
        this._activatedRoute = activatedRoute;
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this._activatedRoute.params
            .pipe(takeUntil(this._destroy))
            .subscribe((params) => {
                this._mediaService.getMediaById(params['id'])
                    .pipe(takeUntil(this._destroy))
                    .subscribe((media: IMedia) => {
                        this.media = media;
                    });
        });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
