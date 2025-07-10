import { Injectable } from '@angular/core';
import { Observable, of, Subject, tap } from 'rxjs';
import { SonarrRepository } from './sonarr.repository';
import { ISonarrActivity } from './types/sonarr-activity.type';
import { WebSocketService } from '../websocket-service/web-socket.service';
import { WebSocketKey } from '../websocket-service/types/web-socket.key';
import { SonarrMapper } from './sonarr.mapper';

@Injectable()
export class SonarrService {

    public activity: Subject<ISonarrActivity> = new Subject<ISonarrActivity>();

    private _activity: ISonarrActivity | null = null;

    private _repository: SonarrRepository;
    private _webSocketService: WebSocketService;

    constructor(repository: SonarrRepository) {
        this._repository = repository;
        this._webSocketService = WebSocketService.instance();
    }

    public ngOnInit(): void {
        this._webSocketService.subscribe(WebSocketKey.SonarrActivity, (payload: any) => {
            this.handleNewActivity(payload);
        });
    }

    public getActivity(): Observable<ISonarrActivity> {
        if (this._activity !== null) {
            return of(this._activity);
        }

        return this._repository.getActivity()
            .pipe(tap((activity: ISonarrActivity) => {
                this._activity = activity;
                this.activity.next(this._activity);
            }));
    }

    public handleNewActivity(payload: any): void {
        this._activity = SonarrMapper.mapWebsocketActivities(payload);
        this.activity.next(this._activity);
    }

    public ngOnDestroy(): void {
        this._webSocketService.unsubscribe(WebSocketKey.SonarrActivity);
    }

}