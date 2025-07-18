import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { IMovie } from '../../services/media-service/types/movie.type';
import { IShow } from '../../services/media-service/types/show.type';

@Component({
    selector: 'home-page',
    templateUrl: './home-page.component.html',
    styleUrls: ['./home-page.component.scss'],
    standalone: false
})
export class HomePageComponent implements OnInit, OnDestroy {

    public movies: Array<IMovie> = [];
    public shows: Array<IShow> = [];
    public isImporting: boolean = false;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;

    constructor(mediaService: MediaService) {
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this._mediaService.getAllMoviesFor('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe((movies) => {
                this.movies = movies;
            });

        this._mediaService.getAllShowsFor('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe((shows) => {
                this.shows = shows;
            });
    }

    public importFromTrakt(): void {
        this.isImporting = true;
        this._mediaService.importFromTrakt('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe(() => {
                this.isImporting = false;
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
