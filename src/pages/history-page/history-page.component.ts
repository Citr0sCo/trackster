import {Component, HostListener, OnDestroy, OnInit} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import {IMedia} from "../../services/media-service/types/media.type";
import {EventService} from "../../services/event-service/event.service";

@Component({
    selector: 'history-page',
    templateUrl: './history-page.component.html',
    styleUrls: ['./history-page.component.scss'],
    standalone: false
})
export class HistoryPageComponent implements OnInit, OnDestroy {

    public media: Record<string, IMedia[]> | null = null;
    public keys: Array<string> = [];
    public visibleKeys: Array<string> = [];
    private _pageSize = 10;
    private _currentPage = 0;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;
    private readonly _eventService: EventService;

    constructor(mediaService: MediaService, eventService: EventService) {
        this._mediaService = mediaService;
        this._eventService = eventService;
    }

    public ngOnInit(): void {
        this._mediaService.getHistoryForUser('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe((media) => {
                this.media = this.groupByDay(media, 'watchedAt');
                this.keys = Object.keys(this.media);
                this.loadMore();
            });

        this._eventService.scrolledToBottom
            .pipe(takeUntil(this._destroy))
            .subscribe((scrolledToBottom) => {
                if (scrolledToBottom) {
                    this.loadMore();
                }
            });
    }

    public groupByDay<T extends Record<string, any>>(items: T[], dateKey: keyof T): Record<string, T[]> {
        return items.reduce((groups, item) => {
            const dateStr = new Date(item[dateKey]).toISOString().split('T')[0]; // YYYY-MM-DD
            if (!groups[dateStr]) {
                groups[dateStr] = [];
            }
            groups[dateStr].push(item);
            return groups;
        }, {} as Record<string, T[]>);
    }

    public loadMore() {
        const nextItems = this.keys.slice(this._currentPage * this._pageSize, (this._currentPage + 1) * this._pageSize);
        this.visibleKeys = [...this.visibleKeys, ...nextItems];
        this._currentPage++;
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
