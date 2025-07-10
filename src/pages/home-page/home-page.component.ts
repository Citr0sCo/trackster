import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { ConfigsService } from '../../services/configs-service/configs.service';
import { IConfigs } from '../../services/configs-service/types/configs.type';

@Component({
    selector: 'home-page',
    templateUrl: './home-page.component.html',
    styleUrls: ['./home-page.component.scss'],
    standalone: false
})
export class HomePageComponent implements OnInit, OnDestroy {

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _configsService: ConfigsService;
    private _configs: IConfigs | null = null;

    constructor(configsService: ConfigsService) {
        this._configsService = configsService;
    }

    public ngOnInit(): void {

        this._configsService.getAllConfigs()
            .subscribe((configs) => {
                this._configs = configs;
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
