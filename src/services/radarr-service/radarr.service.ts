import { Injectable } from '@angular/core';
import { Observable, of, Subject, tap } from 'rxjs';
import { RadarrRepository } from './radarr.repository';
import { IRadarrActivity } from './types/radarr-activity.type';
import { WebSocketService } from '../websocket-service/web-socket.service';
import { WebSocketKey } from '../websocket-service/types/web-socket.key';
import { RadarrMapper } from './radarr.mapper';

@Injectable()
export class RadarrService {

    public activity: Subject<IRadarrActivity> = new Subject<IRadarrActivity>();

    private _activity: IRadarrActivity | null = null;

    private _repository: RadarrRepository;
    private _webSocketService: WebSocketService;

    constructor(repository: RadarrRepository) {
        this._repository = repository;
        this._webSocketService = WebSocketService.instance();
    }

    public ngOnInit(): void {
        this._webSocketService.subscribe(WebSocketKey.RadarrActivity, (payload: any) => {
            this.handleNewActivity(payload);
        });
    }

    public getActivity(): Observable<IRadarrActivity> {
        if (this._activity !== null) {
            return of(this._activity);
        }

        return this._repository.getActivity()
            .pipe(tap((activity: IRadarrActivity) => {
                this._activity = activity;
                this.activity.next(this._activity);
            }));
    }

    public handleNewActivity(payload: any): void {
        this._activity = RadarrMapper.mapWebsocketActivities(payload);
        this.activity.next(this._activity);
    }

    public ngOnDestroy(): void {
        this._webSocketService.unsubscribe(WebSocketKey.RadarrActivity);
    }

}