import { Injectable } from '@angular/core';
import { Observable, of, Subject, tap } from 'rxjs';
import { BuildRepository } from './build.repository';
import { IBuild } from './types/build.type';
import { WebSocketService } from '../websocket-service/web-socket.service';
import { WebSocketKey } from '../websocket-service/types/web-socket.key';
import { BuildMapper } from './build.mapper';

@Injectable()
export class BuildService {

    public builds: Subject<Array<IBuild>> = new Subject<Array<IBuild>>();

    private _buildRepository: BuildRepository;

    constructor(deployRepository: BuildRepository) {
        this._buildRepository = deployRepository;
    }

    public updateAllDockerApps(): Observable<string> {
        return this._buildRepository.updateAllDockerApps();
    }

}