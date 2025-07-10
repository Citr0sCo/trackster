import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { LinkService } from '../../services/link-service/link.service';
import { ILink } from '../../services/link-service/types/link.type';
import { IStatResponse } from '../../services/stats-service/types/stat.response';
import { StatService } from '../../services/stats-service/stat.service';
import { IBuild } from '../../services/build-service/types/build.type';
import { WebSocketService } from '../../services/websocket-service/web-socket.service';
import { WebSocketKey } from '../../services/websocket-service/types/web-socket.key';
import { IStatModel } from '../../services/stats-service/types/stat-model.type';

@Component({
    selector: 'links',
    templateUrl: './links.component.html',
    styleUrls: ['./links.component.scss'],
    standalone: false
})
export class LinksComponent implements OnInit, OnDestroy {

    public mediaLinks: Array<ILink> = [];
    public systemLinks: Array<ILink> = [];
    public productivityLinks: Array<ILink> = [];
    public toolsLinks: Array<ILink> = [];
    public currentTime: Date = new Date();
    public builds: Array<IBuild> = [];
    public isEditModeEnabled: boolean = false;
    public allStats: Array<IStatModel> = new Array<IStatModel>();
    public refreshCache: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    public showWidgets: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    private readonly _linkService: LinkService;
    private readonly _statService: StatService;
    private readonly _webSocketService: WebSocketService;
    private readonly _destroy: Subject<void> = new Subject();

    constructor(linkService: LinkService, statService: StatService) {
        this._linkService = linkService;
        this._statService = statService;
        this._webSocketService = WebSocketService.instance();
    }

    public ngOnInit(): void {
        this._statService.getAll()
            .pipe(takeUntil(this._destroy))
            .subscribe((response: IStatResponse | null) => {
                this.allStats = response?.stats ?? new Array<IStatModel>();
            });

        this._statService.stats
            .asObservable()
            .subscribe((response: IStatResponse | null) => {
                this.allStats = response?.stats ?? new Array<IStatModel>();
            });

        this._linkService.getAllLinks()
            .pipe(
                takeUntil(this._destroy),
                switchMap(() => {
                    return this._linkService.getMediaLinks()
                        .pipe(
                            takeUntil(this._destroy),
                            tap((links) => {
                                this.mediaLinks = links;
                            })
                        );
                }),
                switchMap(() => {
                    return this._linkService.getSystemLinks()
                        .pipe(
                            takeUntil(this._destroy),
                            tap((links) => {
                                this.systemLinks = links;
                            })
                        );
                }),
                switchMap(() => {
                    return this._linkService.getProductivityLinks()
                        .pipe(
                            takeUntil(this._destroy),
                            tap((links) => {
                                this.productivityLinks = links;
                            })
                        );
                }),
                switchMap(() => {
                    return this._linkService.getToolsLinks()
                        .pipe(
                            takeUntil(this._destroy),
                            tap((links) => {
                                this.toolsLinks = links;
                            })
                        );
                })
            )
            .subscribe();

        const showWidgets = localStorage.getItem('showWidgets');
        if (showWidgets !== null) {
            this.showWidgets.next(showWidgets === 'true');
        }

        this.showWidgets
            .pipe(takeUntil(this._destroy))
            .subscribe((showWidgets) => {
                localStorage.setItem('showWidgets', showWidgets.toString());
            });

        setInterval(() => {
            this.currentTime = new Date();
        }, 1000);

        this._webSocketService.send(WebSocketKey.Handshake, { Test: 'Hello World!' });

        this._statService.ngOnInit();
    }

    public getLastSortOrder(links: Array<ILink>): string {

        if (links.length === 0) {
            return 'A';
        }

        const alphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'Z'];

        const lastItemSortOrder = links[links.length - 1].sortOrder;
        const sortOrderCharacters = lastItemSortOrder.split('');
        const lastLetter = sortOrderCharacters[sortOrderCharacters.length - 1];

        const lastLetterOfLastTimeIndex = alphabet.indexOf(lastLetter);

        if (lastLetterOfLastTimeIndex === (alphabet.length - 1)) {
            return `${lastItemSortOrder}A`;
        }

        const lastIndexWithLastCharacter = lastItemSortOrder.slice(0, -1);

        return lastIndexWithLastCharacter + alphabet[lastLetterOfLastTimeIndex + 1];
    }

    public refreshLinkCache(): void {

        this.refreshCache.next(true);

        this._linkService.getUpdatedLinks()
            .pipe(
                takeUntil(this._destroy),
                switchMap(() => {
                    return this._linkService.getMediaLinks()
                        .pipe(
                            takeUntil(this._destroy),
                            tap((links) => {
                                this.mediaLinks = links;
                            })
                        );
                }),
                switchMap(() => {
                    return this._linkService.getSystemLinks()
                        .pipe(
                            takeUntil(this._destroy),
                            tap((links) => {
                                this.systemLinks = links;
                            })
                        );
                }),
                switchMap(() => {
                    return this._linkService.getProductivityLinks()
                        .pipe(
                            takeUntil(this._destroy),
                            tap((links) => {
                                this.productivityLinks = links;
                            })
                        );
                }),
                switchMap(() => {
                    return this._linkService.getToolsLinks()
                        .pipe(
                            takeUntil(this._destroy),
                            tap((links) => {
                                this.toolsLinks = links;
                            })
                        );
                })
            )
            .subscribe();

        this._linkService.refreshCache()
            .subscribe();
    }

    public createColumn(): void {
        this._linkService.createColumn()
            .pipe(takeUntil(this._destroy))
            .subscribe();
    }

    public toggleWidgets(): void {
        this.showWidgets.next(!this.showWidgets.value);
    }

    public ngOnDestroy(): void {
        this._statService.ngOnDestroy();

        this._destroy.next();
    }
}
