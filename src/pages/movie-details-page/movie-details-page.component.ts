import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {MediaService} from "../../services/media-service/media.service";
import {Subject, takeUntil, zip} from "rxjs";
import {IMedia} from "../../services/media-service/types/media.type";
import {IMovie} from "../../services/media-service/types/movie.type";
import {IWatchedEpisode} from "../../services/media-service/types/watched-episode.type";

@Component({
    selector: 'movie-details-page',
    templateUrl: './movie-details-page.component.html',
    styleUrls: ['./movie-details-page.component.scss'],
    standalone: false
})
export class MovieDetailsPageComponent implements OnInit, OnDestroy {

    public movie: IMovie | null = null;
    public watchHistory: Array<IWatchedEpisode> = [];

    public isLoading: boolean = false;

    private readonly _destroy: Subject<void> = new Subject();
    private _activatedRoute: ActivatedRoute;
    private _mediaService: MediaService;

    constructor(activatedRoute: ActivatedRoute, mediaService: MediaService) {
        this._activatedRoute = activatedRoute;
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this.isLoading = true;

        this._activatedRoute.params
            .pipe(takeUntil(this._destroy))
            .subscribe((params) => {

                const movie = this._mediaService.getMovieBySlug(params['slug']);
                const watchHistory = this._mediaService.getMovieWatchHistoryById('citr0s', params['slug']);

                zip(movie, watchHistory)
                    .pipe(takeUntil(this._destroy))
                    .subscribe(([movie, watchHistory]) => {
                        this.isLoading = false;
                        this.movie = movie;
                        this.watchHistory = watchHistory;
                    });
        });
    }

    public redirectBack():void {
        window.history.back();
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
