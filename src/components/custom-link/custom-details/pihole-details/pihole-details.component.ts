import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { ILink } from '../../../../services/link-service/types/link.type';
import { IPiHoleActivity } from '../../../../services/pihole-service/types/pihole-activity.type';
import { PiHoleService } from '../../../../services/pihole-service/pi-hole.service';

@Component({
    selector: 'pihole-details',
    templateUrl: './pihole-details.component.html',
    styleUrls: ['./pihole-details.component.scss'],
    standalone: false
})
export class PiholeDetailsComponent implements OnInit, OnDestroy {

    @Input()
    public item: ILink | null = null;

    public activity: IPiHoleActivity | null = null;
    public formattedQueriesTotal: string | null = null;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _piholeService: PiHoleService;

    constructor(piholeService: PiHoleService) {
        this._piholeService = piholeService;
    }

    public ngOnInit() {
        this._piholeService.getActivity(this.item?.identifier!)
            .pipe(takeUntil(this._destroy))
            .subscribe((activity: IPiHoleActivity) => {
                this.activity = activity;
                const formattedTotal = new Intl.NumberFormat('en-GB');
                this.formattedQueriesTotal = formattedTotal.format(this.activity.queriesToday);
            });

        this._piholeService.activities
            .asObservable()
            .pipe(takeUntil(this._destroy))
            .subscribe((response: Array<IPiHoleActivity>) => {
                this.activity = response.find((x) => x.identifier === this.item?.identifier) ?? null;
                const formattedTotal = new Intl.NumberFormat('en-GB');
                this.formattedQueriesTotal = formattedTotal.format(this.activity?.queriesToday ?? 0);
            });

        this._piholeService.ngOnInit();
    }

    public ngOnDestroy(): void {
        this._piholeService.ngOnDestroy();

        this._destroy.next();
    }
}
