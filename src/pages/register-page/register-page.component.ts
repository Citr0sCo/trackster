import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from "../../services/media-service/media.service";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuthenticationService } from "../../services/authentication-service/authentication.service";
import { Provider } from "../../core/providers.enum";
import { IRegisterRequest } from "../../services/authentication-service/types/register.request";
import { IRegisterResponse } from "../../services/authentication-service/types/register.response";
import { SettingsService } from '../../services/settings-service/settings.service';
import { ISettings } from '../../services/settings-service/types/settings.type';

@Component({
    selector: 'register-page',
    templateUrl: './register-page.component.html',
    styleUrls: ['./register-page.component.scss'],
    standalone: false
})
export class RegisterPageComponent implements OnInit, OnDestroy {

    public isLoading: boolean = false;
    public activePoster: string | null = null;
    public formGroup: FormGroup = new FormGroup<any>({
        username: new FormControl('', [Validators.required]),
        email: new FormControl('', [Validators.required, Validators.email]),
        password: new FormControl('', [Validators.required]),
        passwordConfirm: new FormControl('', [Validators.required]),
    });
    public response: IRegisterResponse | null = null;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;
    private readonly _authService: AuthenticationService;
    private readonly _settingsService: SettingsService;

    private _settings: ISettings | null = null;

    constructor(mediaService: MediaService, authService: AuthenticationService, settingsService: SettingsService) {
        this._mediaService = mediaService;
        this._authService = authService;
        this._settingsService = settingsService;
    }

    public ngOnInit(): void {
        this._mediaService.getPosters()
            .pipe(takeUntil(this._destroy))
            .subscribe((posters: Array<string>) => {
                this.activePoster = posters[this.randomIntFromInterval(0, posters.length - 1)]
            });

        this._settingsService
            .getSettings()
            .subscribe((settings) => {
                this._settings = settings;
            });

        if (this._authService.isLoggedIn()) {
            window.location.href = "/app/home";
        }
    }

    public register(): void {
        this.isLoading = true;

        const username = this.formGroup.controls['username'].value;
        const email = this.formGroup.controls['email'].value;
        const password = this.formGroup.controls['password'].value;
        const passwordConfirm = this.formGroup.controls['passwordConfirm'].value;

        const request: IRegisterRequest = {
            username: username,
            email: email,
            password: password
        };

        this._authService.register(Provider.Email, request)
            .pipe(takeUntil(this._destroy))
            .subscribe((response) => {
                this.isLoading = false;

                this.response = response;

                if (response.hasError == false) {
                    window.location.href = "/";
                }
            });
    }

    private randomIntFromInterval(min: number, max: number): number {
        return Math.floor(Math.random() * (max - min + 1) + min);
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
