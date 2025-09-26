import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Subject, takeUntil, zip} from "rxjs";
import {ShowService} from "../../services/show-service/show.service";
import {IShow} from "../../services/show-service/types/show.type";
import {ISeason} from "../../services/show-service/types/season.type";
import {IEpisode} from "../../services/show-service/types/episode.type";
import {IWatchedEpisode} from "../../services/show-service/types/watched-episode.type";
import {UserService} from "../../services/user-service/user.service";
import {IUser} from "../../services/user-service/types/user.type";

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
    private readonly _activatedRoute: ActivatedRoute;
    private readonly _showService: ShowService;
    private readonly _userService: UserService;

    private slug: string = '';
    private seasonId: number = 0;
    private episodeId: number = 0;

    constructor(activatedRoute: ActivatedRoute, showService: ShowService, userService: UserService) {
        this._activatedRoute = activatedRoute;
        this._showService = showService;
        this._userService = userService;
    }

    public ngOnInit(): void {
        this.isLoading = true;

        this._activatedRoute.params
            .pipe(takeUntil(this._destroy))
            .subscribe((params) => {

                this._userService.getUserBySession()
                    .pipe(takeUntil(this._destroy))
                    .subscribe((user: IUser) => {
                        const episode = this._showService.getEpisodeByNumber(params['slug'], params['seasonId'], params['episodeId']);
                        const watchHistory = this._showService.getEpisodeWatchHistory(user.username, params['slug'], params['seasonId'], params['episodeId']);

                        this.slug = params['slug'];
                        this.seasonId = params['seasonId'];
                        this.episodeId = params['episodeId'];

                        zip(episode, watchHistory)
                            .pipe(takeUntil(this._destroy))
                            .subscribe(([episode, watchHistory]) => {
                                this.isLoading = false;
                                this.episode = episode;
                                this.season = episode.season;
                                this.show = episode.season.show;
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
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
