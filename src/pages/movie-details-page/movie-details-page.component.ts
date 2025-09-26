import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Subject, takeUntil, zip} from "rxjs";
import {MovieService} from "../../services/movie-service/movie.service";
import {IMovie} from "../../services/movie-service/types/movie.type";
import {IWatchedMovie} from "../../services/movie-service/types/watched-movie.type";
import {UserService} from "../../services/user-service/user.service";

@Component({
    selector: 'movie-details-page',
    templateUrl: './movie-details-page.component.html',
    styleUrls: ['./movie-details-page.component.scss'],
    standalone: false
})
export class MovieDetailsPageComponent implements OnInit, OnDestroy {

    public movie: IMovie | null = null;
    public watchHistory: Array<IWatchedMovie> = [];

    public isLoading: boolean = false;
    public isUpdating: boolean = false;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _activatedRoute: ActivatedRoute;
    private readonly _movieService: MovieService;
    private readonly _userService: UserService;

    constructor(activatedRoute: ActivatedRoute, movieService: MovieService, userService: UserService) {
        this._activatedRoute = activatedRoute;
        this._movieService = movieService;
        this._userService = userService;
    }

    public ngOnInit(): void {
        this.isLoading = true;

        this._activatedRoute.params
            .pipe(takeUntil(this._destroy))
            .subscribe((params) => {

                this._userService.getUserBySession()
                    .pipe(takeUntil(this._destroy))
                    .subscribe((user) => {
                        const movie = this._movieService.getMovieBySlug(params['slug']);
                        const watchHistory = this._movieService.getMovieWatchHistoryById(user.username, params['slug']);

                        zip(movie, watchHistory)
                            .pipe(takeUntil(this._destroy))
                            .subscribe(([movie, watchHistory]) => {
                                this.isLoading = false;
                                this.movie = movie;
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
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
