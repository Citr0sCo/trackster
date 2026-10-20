import {Component, OnDestroy, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Subject, takeUntil, zip} from "rxjs";
import {ShowService} from "../../services/show-service/show.service";
import {IShow} from "../../services/show-service/types/show.type";
import {ISeason} from "../../services/show-service/types/season.type";
import {IEpisode} from "../../services/show-service/types/episode.type";
import {IWatchedEpisode} from "../../services/show-service/types/watched-episode.type";
import {UserService} from "../../services/user-service/user.service";
import {IUser} from "../../services/user-service/types/user.type";
import {IMediaDetails} from "../../services/media-service/types/media-details.type";
import {MediaService} from "../../services/media-service/media.service";

@Component({
    selector: 'show-details-page',
    templateUrl: './show-details-page.component.html',
    styleUrls: ['./show-details-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ShowDetailsPageComponent implements OnInit, OnDestroy {

    public show: IShow | null = null;
    public season: ISeason | null = null;
    public episode: IEpisode | null = null;
    public watchHistory: Array<IWatchedEpisode> = [];
    public details: IMediaDetails | null = null;

    public isLoading: boolean = false;
    public isUpdating: boolean = false;

    public isRemoveDialogOpen: boolean = false;
    public isRemovingFromHistory: boolean = false;

    private username: string = '';

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _activatedRoute: ActivatedRoute;
    private readonly _showService: ShowService;
    private readonly _userService: UserService;
    private readonly _mediaService: MediaService;

    private slug: string = '';
    private seasonId: number = 0;
    private episodeId: number = 0;

    constructor(activatedRoute: ActivatedRoute, showService: ShowService, userService: UserService, mediaService: MediaService) {
        this._activatedRoute = activatedRoute;
        this._showService = showService;
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
                    .subscribe((user: IUser) => {
                        this.username = user.username;

                        const episode = this._showService.getEpisodeByNumber(params['slug'], params['seasonId'], params['episodeId']);
                        const details = this._showService.getShowDetailsBySlug(params['slug']);
                        const watchHistory = this._showService.getEpisodeWatchHistory(user.username, params['slug'], params['seasonId'], params['episodeId']);

                        this.slug = params['slug'];
                        this.seasonId = params['seasonId'];
                        this.episodeId = params['episodeId'];

                        zip(episode, details, watchHistory)
                            .pipe(takeUntil(this._destroy))
                            .subscribe(([episode, details, watchHistory]) => {
                                this.isLoading = false;
                                this.episode = episode;
                                this.season = episode.season;
                                this.show = episode.season.show;
                                this.details = details;
                                this.watchHistory = watchHistory;
                            });
                    });
        });
    }

    public redirectBack():void {
        window.history.back();
    }

    public updateEpisode(): void {
        this.isUpdating = true;

        this._showService
            .updateEpisodeById(this.slug, this.seasonId, this.episodeId)
            .pipe(takeUntil(this._destroy))
            .subscribe((episode) => {
                this.isUpdating = false;
                this.episode = episode;
                this.season = episode.season;
                this.show = episode.season.show;
                this._showService.getShowDetailsBySlug(this.slug)
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

        this._showService.removeFromHistory(this.slug, this.seasonId, this.episodeId)
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
        this._showService.getEpisodeWatchHistory(this.username, this.slug, this.seasonId, this.episodeId)
            .pipe(takeUntil(this._destroy))
            .subscribe((watchHistory) => this.watchHistory = watchHistory);
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
