import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { ILink } from '../../../../services/link-service/types/link.type';
import { IPlexSession } from '../../../../services/plex-service/types/plex-session.type';
import { PlexService } from '../../../../services/plex-service/plex.service';

@Component({
    selector: 'plex-details',
    templateUrl: './plex-details.component.html',
    styleUrls: ['./plex-details.component.scss'],
    standalone: false
})
export class PlexDetailsComponent implements OnInit, OnDestroy {

    @Input()
    public item: ILink | null = null;

    public plexSessions: Array<IPlexSession> = [];

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _plexService: PlexService;

    constructor(plexService: PlexService) {
        this._plexService = plexService;
    }

    public ngOnInit() {
        this._plexService.getActivity()
            .pipe(takeUntil(this._destroy))
            .subscribe((response: Array<IPlexSession>) => {
                this.plexSessions = response;
            });

        this._plexService.sessions
            .asObservable()
            .pipe(takeUntil(this._destroy))
            .subscribe((response: Array<IPlexSession>) => {
                this.plexSessions = response;
            });

        setInterval(() => {
            for (const session of this.plexSessions) {
                if (session.state === 'playing') {
                    session.viewOffset += 1000;
                }
            }
        }, 1000);

        this._plexService.ngOnInit();
    }

    public getTimeFromDuration(duration: number): string {

        const date = new Date(duration);

        let displayText = '';

        const hours = date.getUTCHours();
        if (hours > 0) {
            displayText += `${hours}:`;
        }

        const minutes = date.getMinutes();
        if (minutes < 10) {
            displayText += `0${minutes}:`;
        } else {
            displayText += `${minutes}:`;
        }

        const seconds = date.getSeconds();
        if (seconds < 10) {
            displayText += `0${seconds}`;
        } else {
            displayText += `${seconds}`;
        }

        return displayText;
    }

    public trimTitle(title: string): string {
        if (title.length > 18) {
            return `${title.slice(0, 18).trim()}...`;
        }
        return title;
    }

    public ngOnDestroy(): void {
        this._plexService.ngOnDestroy();

        this._destroy.next();
    }
}
