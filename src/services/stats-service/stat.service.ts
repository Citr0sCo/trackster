import { Injectable } from '@angular/core';
import { Observable, of, Subject, tap } from 'rxjs';
import { StatRepository } from './stat.repository';
import { IStatResponse } from './types/stat.response';
import { WebSocketService } from '../websocket-service/web-socket.service';
import { WebSocketKey } from '../websocket-service/types/web-socket.key';
import { StatMapper } from './stat.mapper';
import { IDockerAppUpdateProgressResponse } from './types/docker-app-update-progress-response.response';

@Injectable()
export class StatService {

    public stats: Subject<IStatResponse | null> = new Subject<IStatResponse | null>();
    public dockerAppUpdateProgress: Subject<IDockerAppUpdateProgressResponse | null> = new Subject<IDockerAppUpdateProgressResponse | null>();

    private _statsCache: IStatResponse | null = null;

    private _statRepository: StatRepository;
    private _webSocketService: WebSocketService;

    constructor(deployRepository: StatRepository) {
        this._statRepository = deployRepository;
        this._webSocketService = WebSocketService.instance();
    }

    public ngOnInit(): void {
        this._webSocketService.subscribe(WebSocketKey.ServerStats, (payload: any) => {
            this.handleNewStats(payload);
        });
        this._webSocketService.subscribe(WebSocketKey.DockerAppUpdateProgress, (payload: any) => {
            this.handleDockerAppUpdateProgress(payload);
        });
    }

    public getAll(): Observable<IStatResponse> {
        if (this._statsCache !== null) {
            return of(this._statsCache);
        }

        return this._statRepository.getAll()
            .pipe(tap((deploys: IStatResponse) => {
                this._statsCache = deploys;
            }));
    }

    public handleNewStats(payload: any): void {

        if (payload.Stats.length === 0) {
            return;
        }

        this._statsCache = StatMapper.map(payload);
        this.stats.next(this._statsCache);
    }

    public handleDockerAppUpdateProgress(payload: any): void {
        this.dockerAppUpdateProgress.next({
            result: payload.Result,
            finished: payload.Finished
        });
    }

    public ngOnDestroy(): void {
        this._webSocketService.unsubscribe(WebSocketKey.ServerStats);
        this._webSocketService.unsubscribe(WebSocketKey.DockerAppUpdateProgress);
    }
}