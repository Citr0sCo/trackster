import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { IMovie } from '../../services/media-service/types/movie.type';
import { IShow } from '../../services/media-service/types/show.type';
import { IMedia, MediaType } from "../../services/media-service/types/media.type";
import { MediaMapper } from "../../services/media-service/media.mapper";
import { IWatchedMovie } from "../../services/media-service/types/watched-movie.type";
import { IWatchedShow } from "../../services/media-service/types/watched-show.type";
import { UserService } from '../../services/user-service/user.service';
import { AuthenticationService } from '../../services/authentication-service/authentication.service';
import {MovieService} from "../../services/movie-service/movie.service";
import {ShowService} from "../../services/show-service/show.service";

@Component({
    selector: 'home-page',
    templateUrl: './home-page.component.html',
    styleUrls: ['./home-page.component.scss'],
    standalone: false
})
export class HomePageComponent implements OnInit, OnDestroy {

    public movies: Array<IWatchedMovie> = [];
    public shows: Array<IWatchedShow> = [];

    public moviesLoading: boolean = false;
    public showsLoading: boolean = false;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _userService: UserService;
    private _movieService: MovieService;
    private _showService: ShowService;

    constructor(userService: UserService, movieService: MovieService, showService: ShowService) {
        this._userService = userService;
        this._movieService = movieService;
        this._showService = showService;
    }

    public ngOnInit(): void {
        this.moviesLoading = true;
        this.showsLoading = true;

        this._userService.getUserBySession()
            .pipe(takeUntil(this._destroy))
            .subscribe((user) => {
                this._movieService.getAllMoviesFor(user.username)
                    .pipe(takeUntil(this._destroy))
                    .subscribe((movies) => {
                        this.movies = movies;
                        this.moviesLoading = false;
                    });

                this._showService.getAllShowsFor(user.username)
                    .pipe(takeUntil(this._destroy))
                    .subscribe((shows) => {
                        this.shows = shows;
                        this.showsLoading = false;
                    });
            });
    }

    public movieToMedia(movie: IWatchedMovie): IMedia {
        return MediaMapper.fromMovie(movie);
    }

    public showToMedia(show: IWatchedShow): IMedia {
        return MediaMapper.fromShow(show);
    }

    public bustCache(): void {
        this._movieService.bustCache();
        this._showService.bustCache();
        this.ngOnInit();
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
