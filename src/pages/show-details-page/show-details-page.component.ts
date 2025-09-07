import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {MediaService} from "../../services/media-service/media.service";
import {Subject, takeUntil, zip} from "rxjs";
import {IShow} from "../../services/media-service/types/show.type";
import {IEpisode} from "../../services/media-service/types/episode.type";
import {ISeason} from "../../services/media-service/types/season.type";
import {IWatchedEpisode} from "../../services/media-service/types/watched-episode.type";

@Component({
    selector: 'show-details-page',
    templateUrl: './show-details-page.component.html',
    styleUrls: ['./show-details-page.component.scss'],
    standalone: false
})
export class ShowDetailsPageComponent implements OnInit, OnDestroy {

    public show: IShow | null = null;
    public season: ISeason | null = null;
    public episode: IEpisode | null = null;
    public watchHistory: Array<IWatchedEpisode> = [];

    public isLoading: boolean = false;
    public isUpdating: boolean = false;

    private readonly _destroy: Subject<void> = new Subject();
    private _activatedRoute: ActivatedRoute;
    private _mediaService: MediaService;
    private slug: string = '';
    private seasonId: number = 0;
    private episodeId: number = 0;

    constructor(activatedRoute: ActivatedRoute, mediaService: MediaService) {
        this._activatedRoute = activatedRoute;
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this.isLoading = true;

        this._activatedRoute.params
            .pipe(takeUntil(this._destroy))
            .subscribe((params) => {

                const show = this._mediaService.getShowBySlug(params['slug']);
                const season = this._mediaService.getSeasonByNumber(params['slug'], params['seasonId']);
                const episode = this._mediaService.getEpisodeByNumber(params['slug'], params['seasonId'], params['episodeId']);
                const watchHistory = this._mediaService.getEpisodeWatchHistory('citr0s', params['slug'], params['seasonId'], params['episodeId']);

                this.slug = params['slug'];
                this.seasonId = params['seasonId'];
                this.episodeId = params['episodeId'];

                zip(show, season, episode, watchHistory)
                    .pipe(takeUntil(this._destroy))
                    .subscribe(([show, season, episode, watchHistory]) => {
                        this.isLoading = false;
                        this.show = show;
                        this.season = season;
                        this.episode = episode;
                        this.watchHistory = watchHistory;
                    });
        });
    }

    public redirectBack():void {
        window.history.back();
    }

    public updateEpisode(): void {
        this.isUpdating = true;

        const episode = this._mediaService
            .updateEpisodeById(this.slug, this.seasonId, this.episodeId)
            .pipe(takeUntil(this._destroy))
            .subscribe((episode) => {
                this.isUpdating = false;

                this.episode!.title = episode.title;
                this.episode!.title = episode.title;
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }

    protected readonly pageXOffset = pageXOffset;
}
