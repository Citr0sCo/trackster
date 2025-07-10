import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IStatResponse } from '../../services/stats-service/types/stat.response';
import { StatService } from '../../services/stats-service/stat.service';
import { IStatModel } from '../../services/stats-service/types/stat-model.type';

@Component({
    selector: 'top-info',
    templateUrl: './top-info.component.html',
    styleUrls: ['./top-info.component.scss'],
    standalone: false
})
export class TopInfoComponent implements OnInit, OnDestroy {

    public allStats: Array<IStatModel> = new Array<IStatModel>();

    private readonly _subscriptions: Subscription = new Subscription();
    private readonly _statService: StatService;

    constructor(statService: StatService) {
        this._statService = statService;
    }

    public ngOnInit(): void {

        this._subscriptions.add(
            this._statService.getAll()
                .subscribe((response: IStatResponse | null) => {
                    this.allStats = response?.stats ?? new Array<IStatModel>();
                })
        );

        this._subscriptions.add(
            this._statService.stats
                .asObservable()
                .subscribe((response: IStatResponse | null) => {
                    this.allStats = response?.stats ?? new Array<IStatModel>();
                })
        );

        this._statService.ngOnInit();
    }

    public ngOnDestroy(): void {
        this._statService.ngOnDestroy();

        this._subscriptions.unsubscribe();
    }
}
