import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { ILink } from '../../../../services/link-service/types/link.type';
import { SonarrService } from '../../../../services/sonarr-service/sonarr.service';
import { ISonarrActivity, ISonarrHealth } from '../../../../services/sonarr-service/types/sonarr-activity.type';

@Component({
    selector: 'sonarr-details',
    templateUrl: './sonarr-details.component.html',
    styleUrls: ['./sonarr-details.component.scss'],
    standalone: false
})
export class SonarrDetailsComponent implements OnInit, OnDestroy {

    @Input()
    public item: ILink | null = null;

    public activity: ISonarrActivity | null = null;
    public readonly Object = Object;
    public groupedHealth: any | null = null;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _sonarrService: SonarrService;

    constructor(sonarrService: SonarrService) {
        this._sonarrService = sonarrService;
    }

    public ngOnInit() {
        this._sonarrService.getActivity()
            .pipe(takeUntil(this._destroy))
            .subscribe((activity: ISonarrActivity) => {
                this.activity = activity;
                // @ts-ignore
                this.groupedHealth = Object.groupBy(this.activity.health, (x: any) => x.type);
            });

        this._sonarrService.activity
            .asObservable()
            .pipe(takeUntil(this._destroy))
            .subscribe((activity: ISonarrActivity) => {
                this.activity = activity;
                // @ts-ignore
                this.groupedHealth = Object.groupBy(this.activity.health, (x: any) => x.type);
            });

        this._sonarrService.ngOnInit();
    }

    public getNumberOfType(healthType: string): number {
        return this.activity?.health?.filter((x) => x.type === healthType)?.length ?? 0;
    }

    public getTitle(problems: Array<ISonarrHealth>): string {

        let message = '';

        for (const problem of problems) {
            message += `${problem.message}\n\n`;
        }

        return message.slice(0, message.length - 2);
    }

    public ngOnDestroy(): void {
        this._sonarrService.ngOnDestroy();

        this._destroy.next();
    }
}
