import { Injectable } from '@angular/core';
import { Observable, of, Subject, tap } from 'rxjs';
import { PiHoleRepository } from './pi-hole-repository.service';
import { IPiHoleActivity } from './types/pihole-activity.type';
import { WebSocketService } from '../websocket-service/web-socket.service';
import { WebSocketKey } from '../websocket-service/types/web-socket.key';
import { PiHoleMapper } from './piHoleMapper';

@Injectable()
export class PiHoleService {

    public activities: Subject<Array<IPiHoleActivity>> = new Subject<Array<IPiHoleActivity>>();

    private _activities: Array<IPiHoleActivity> = [];

    private _repository: PiHoleRepository;
    private _webSocketService: WebSocketService;

    constructor(repository: PiHoleRepository) {
        this._repository = repository;
        this._webSocketService = WebSocketService.instance();
    }

    public ngOnInit(): void {
        this._webSocketService.subscribe(WebSocketKey.PiHoleActivity, (payload: any) => {
            this.handleNewActivity(payload);
        });
    }

    public getActivity(identifier: string): Observable<IPiHoleActivity> {
        if (this._activities.length > 0) {
            return of(this._activities.find((x) => x.identifier === identifier)!);
        }

        return this._repository.getActivity(identifier)
            .pipe(tap((activity: IPiHoleActivity) => {
                this._activities = this._activities.map((x) => {

                    if (x.identifier === activity.identifier) {
                        x.queriesToday = activity.queriesToday;
                        x.blockedToday = activity.blockedToday;
                        x.blockedPercentage = activity.blockedPercentage;
                    }

                    return x;
                });
            }));
    }

    public handleNewActivity(payload: any): void {
        this._activities = PiHoleMapper.mapActivities(payload);
        this.activities.next(this._activities);
    }

    public ngOnDestroy(): void {
        this._webSocketService.unsubscribe(WebSocketKey.PlexActivity);
    }

}