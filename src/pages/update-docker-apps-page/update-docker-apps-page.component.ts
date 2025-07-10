import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { StatService } from '../../services/stats-service/stat.service';
import { BuildService } from '../../services/build-service/build.service';
import {
    IDockerAppUpdateProgressResponse
} from '../../services/stats-service/types/docker-app-update-progress-response.response';
import { TerminalParser } from '../../core/terminal-parser';

@Component({
    selector: 'update-docker-apps-page',
    templateUrl: './update-docker-apps-page.component.html',
    styleUrls: ['./update-docker-apps-page.component.scss'],
    standalone: false
})
export class UpdateDockerAppsPageComponent implements OnInit, OnDestroy {

    public updateAllDockerAppsResult: IDockerAppUpdateProgressResponse = { finished: true, result: 'Waiting for log...' };
    public showLog: boolean = false;

    private readonly _statService: StatService;
    private readonly _buildService: BuildService;
    private readonly _statsService: StatService;
    private readonly _destroy: Subject<void> = new Subject();

    constructor(statService: StatService, buildService: BuildService, statsService: StatService) {
        this._statService = statService;
        this._buildService = buildService;
        this._statsService = statsService;
    }

    public ngOnInit(): void {

        this._statService.dockerAppUpdateProgress
            .asObservable()
            .pipe(takeUntil(this._destroy))
            .subscribe((response: IDockerAppUpdateProgressResponse | null) => {

                const parsedOutput = new TerminalParser(response!.result).toHtml();

                if (parsedOutput.length === 0) {
                    this.updateAllDockerAppsResult.finished = true;
                    return;
                }

                this.showLog = true;

                this.updateAllDockerAppsResult = {
                    result: parsedOutput,
                    finished: response!.finished
                };

                setTimeout(() => {
                    const logWindowElement = document.querySelector('.log-window');
                    if (logWindowElement) {
                        logWindowElement.scrollTo(0, logWindowElement.scrollHeight);
                    }
                }, 10);
            });

        this._statsService.ngOnInit();
    }

    public updateAllDockerApps(): void {

        if (!this.updateAllDockerAppsResult.finished) {
            return;
        }

        this._buildService.updateAllDockerApps()
            .subscribe(() => {
                this.showLog = true;
            });
    }

    public ngOnDestroy(): void {
        this._statsService.ngOnDestroy();

        this._destroy.next();
    }
}
