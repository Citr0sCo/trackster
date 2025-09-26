import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {MediaService} from "../../services/media-service/media.service";
import {Subject, takeUntil, zip} from "rxjs";
import {IMovie} from "../../services/media-service/types/movie.type";
import {IWatchedEpisode} from "../../services/media-service/types/watched-episode.type";
import {MovieService} from "../../services/movie-service/movie.service";

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
    public isUpdating: boolean = false;

    private readonly _destroy: Subject<void> = new Subject();
    private _activatedRoute: ActivatedRoute;
    private _movieService: MovieService;

    constructor(activatedRoute: ActivatedRoute, movieService: MovieService) {
        this._activatedRoute = activatedRoute;
        this._movieService = movieService;
    }

    public ngOnInit(): void {
        this.isLoading = true;

        this._activatedRoute.params
            .pipe(takeUntil(this._destroy))
            .subscribe((params) => {

                const movie = this._movieService.getMovieBySlug(params['slug']);
                const watchHistory = this._movieService.getMovieWatchHistoryById('citr0s', params['slug']);

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

    public updateMovie(): void {
        this.isUpdating = true;

        this._movieService
            .updateMovieById(this.movie!.slug)
            .pipe(takeUntil(this._destroy))
            .subscribe((movie) => {
                this.isUpdating = false;

                this.movie!.title = movie.title;
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
