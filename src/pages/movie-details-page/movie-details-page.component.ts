import {Component, OnDestroy, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Subject, takeUntil, zip} from "rxjs";
import {MovieService} from "../../services/movie-service/movie.service";
import {IMovie} from "../../services/movie-service/types/movie.type";
import {IWatchedMovie} from "../../services/movie-service/types/watched-movie.type";
import {UserService} from "../../services/user-service/user.service";
import {IMediaDetails} from "../../services/media-service/types/media-details.type";
import {MediaService} from "../../services/media-service/media.service";

@Component({
    selector: 'movie-details-page',
    templateUrl: './movie-details-page.component.html',
    styleUrls: ['./movie-details-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class MovieDetailsPageComponent implements OnInit, OnDestroy {

    public movie: IMovie | null = null;
    public watchHistory: Array<IWatchedMovie> = [];
    public details: IMediaDetails | null = null;

    public isLoading: boolean = false;
    public isUpdating: boolean = false;

    public isRemoveDialogOpen: boolean = false;
    public isRemovingFromHistory: boolean = false;

    private username: string = '';

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _activatedRoute: ActivatedRoute;
    private readonly _movieService: MovieService;
    private readonly _userService: UserService;
    private readonly _mediaService: MediaService;

    constructor(activatedRoute: ActivatedRoute, movieService: MovieService, userService: UserService, mediaService: MediaService) {
        this._activatedRoute = activatedRoute;
        this._movieService = movieService;
        this._userService = userService;
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this.isLoading = true;

        this._activatedRoute.params
            .pipe(takeUntil(this._destroy))
            .subscribe((params) => {

                this._userService.getUserBySession()
                    .pipe(takeUntil(this._destroy))
                    .subscribe((user) => {
                        this.username = user.username;

                        const movie = this._movieService.getMovieBySlug(params['slug']);
                        const details = this._movieService.getMovieDetailsBySlug(params['slug']);
                        const watchHistory = this._movieService.getMovieWatchHistoryById(user.username, params['slug']);

                        zip(movie, details, watchHistory)
                            .pipe(takeUntil(this._destroy))
                            .subscribe(([movie, details, watchHistory]) => {
                                this.isLoading = false;
                                this.movie = movie;
                                this.details = details;
                                this.watchHistory = watchHistory;
                            });
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
                this.movie = movie;
                this._movieService.getMovieDetailsBySlug(movie.slug)
                    .pipe(takeUntil(this._destroy))
                    .subscribe((details) => this.details = details);
            });
    }

    public requestRemoveFromHistory(): void {
        this.isRemoveDialogOpen = true;
    }

    public cancelRemoveFromHistory(): void {
        this.isRemoveDialogOpen = false;
    }

    public confirmRemoveFromHistory(): void {
        this.isRemovingFromHistory = true;

        this._movieService.removeFromHistory(this.movie!.slug)
            .pipe(takeUntil(this._destroy))
            .subscribe({
                next: () => this.finishRemoval(),
                error: () => this.isRemovingFromHistory = false
            });
    }

    private finishRemoval(): void {
        this.isRemovingFromHistory = false;
        this.isRemoveDialogOpen = false;
        this._mediaService.bustCache();
        this.refreshWatchHistory();
    }

    public formatRuntime(minutes: number): string {
        if (!minutes) {
            return '';
        }

        const hours = Math.floor(minutes / 60);
        const remainingMinutes = minutes % 60;
        return hours ? `${hours}h ${remainingMinutes}m` : `${remainingMinutes}m`;
    }

    private refreshWatchHistory(): void {
        this._movieService.getMovieWatchHistoryById(this.username, this.movie!.slug)
            .pipe(takeUntil(this._destroy))
            .subscribe((watchHistory) => this.watchHistory = watchHistory);
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
