import { Component, OnDestroy, OnInit } from '@angular/core';
import { SettingsService } from '../../services/settings-service/settings.service';
import { ISettings } from '../../services/settings-service/types/settings.type';
import { Subject } from 'rxjs';

@Component({
    selector: 'auth-with-trakt-button',
    templateUrl: './auth-with-trakt-button.component.html',
    styleUrls: ['./auth-with-trakt-button.component.scss'],
    standalone: false
})
export class AuthWithTraktButtonComponent implements OnInit, OnDestroy {

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _settingsService: SettingsService;

    private _settings: ISettings | null = null;

    constructor(settingsService: SettingsService) {
        this._settingsService = settingsService;
    }

    public ngOnInit(): void {
        this._settingsService
            .getSettings()
            .subscribe((settings) => {
                this._settings = settings;
            });
    }

    public authWithTrakt(): void {

        const clientId = this._settings?.traktClientId;
        const redirectUri = `${window.location.origin}/authorize/trakt`;
        const state = encodeURIComponent(JSON.stringify({}));

        window.location.href = `https://api.trakt.tv/oauth/authorize?response_type=code&client_id=${clientId}&redirect_uri=${redirectUri}&state=${state}`;
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
