import {Component, OnDestroy, OnInit} from '@angular/core';
import {Subject, takeUntil} from 'rxjs';
import {MediaService} from '../../services/media-service/media.service';
import {IMovie} from '../../services/media-service/types/movie.type';
import {IShow} from '../../services/media-service/types/show.type';
import {IMedia, MediaType} from "../../services/media-service/types/media.type";
import {MediaMapper} from "../../services/media-service/media.mapper";

@Component({
    selector: 'home-page',
    templateUrl: './home-page.component.html',
    styleUrls: ['./home-page.component.scss'],
    standalone: false
})
export class HomePageComponent implements OnInit, OnDestroy {

    public movies: Array<IMovie> = [];
    public shows: Array<IShow> = [];

    public moviesLoading: boolean = false;
    public showsLoading: boolean = false;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;

    constructor(mediaService: MediaService) {
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this.moviesLoading = true;
        this.showsLoading = true;

        this._mediaService.getAllMoviesFor('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe((movies) => {
                this.movies = movies;
                this.moviesLoading = false;
            });

        this._mediaService.getAllShowsFor('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe((shows) => {
                this.shows = shows;
                this.showsLoading = false;
            });
    }

    public movieToMedia(movie: IMovie) : IMedia {
        return MediaMapper.fromMovie(movie);
    }

    public showToMedia(show: IShow) : IMedia {
        return MediaMapper.fromShow(show);
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
