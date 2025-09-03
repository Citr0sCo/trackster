import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { IMedia, MediaType } from '../../services/media-service/types/media.type';

@Component({
    selector: 'statistics-page',
    templateUrl: './statistics-page.component.html',
    styleUrls: ['./statistics-page.component.scss'],
    standalone: false
})
export class StatisticsPageComponent implements OnInit, OnDestroy {

    public isImporting: boolean = false;
    public username: string = 'citr0s';
    public media: Array<IMedia> = [];
    public totalMovies: number = 0;
    public totalEpisodes: number = 0;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;

    constructor(mediaService: MediaService) {
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this.isImporting = true;

        this._mediaService.getHistoryForUser('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe((media) => {
                this.isImporting = false;

                this.media = media;
                this.totalMovies = this.media.filter((x) => x.mediaType === MediaType.Movie).length;
                this.totalEpisodes = this.media.filter((x) => x.mediaType === MediaType.Episode).length;
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
