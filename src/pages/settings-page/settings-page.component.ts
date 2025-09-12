import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { StreamService } from "../../core/event-service.service";
import { environment } from "../../environments/environment";
import { round } from "@popperjs/core/lib/utils/math";
import { UserService } from '../../services/user-service/user.service';
import { IUser } from '../../services/user-service/types/user.type';

@Component({
    selector: 'settings-page',
    templateUrl: './settings-page.component.html',
    styleUrls: ['./settings-page.component.scss'],
    standalone: false
})
export class SettingsPageComponent implements OnInit, OnDestroy {

    public isImporting: boolean = false;
    public isDebug: boolean = false;
    public progress: Array<string> = [];
    public mediaType: string = '';
    public processed: number = 0;
    public total: number = 0;
    public readonly round = round;
    public user: IUser | null = null;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;
    private readonly _streamService: StreamService;
    private readonly _userService: UserService;

    constructor(mediaService: MediaService, streamService: StreamService, userService: UserService) {
        this._mediaService = mediaService;
        this._streamService = streamService;
        this._userService = userService;
    }

    public ngOnInit(): void {

        this._userService.getUserBySession()
            .pipe(takeUntil(this._destroy))
            .subscribe((user) => {
                this.user = user;
            });
    }

    public importFromTrakt(): void {
        this.isImporting = true;

        this._streamService.startStream(`${environment.apiBaseUrl}/api/media/import`)
            .pipe(takeUntil(this._destroy))
            .subscribe((data) => {
                    const progress = JSON.parse(data);
                    this.progress.push(`${progress.Data}\n`);

                    if (progress.Total > 0) {
                        this.total = progress.Total;
                    }

                    if (progress.Processed > 0) {
                        this.processed = progress.Processed;
                    }

                    if (progress.Type?.length > 0) {
                        this.mediaType = progress.Type;
                    }

                    if (progress.Data?.indexOf('DONE!') > -1) {
                        this.isImporting = false;

                        setTimeout(() => {
                            const logWindowElement = document.querySelector('.log-window');
                            if (logWindowElement) {
                                logWindowElement.scrollTo(0, logWindowElement.scrollHeight);
                            }
                        }, 100);
                    }

                    setTimeout(() => {
                        const logWindowElement = document.querySelector('.log-window');
                        if (logWindowElement) {
                            logWindowElement.scrollTo(0, logWindowElement.scrollHeight);
                        }
                    }, 100);

                }, (error) => {
                    console.error(error);
                }
            );
    }

    public saveSettings(): void {

    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
