import {Component, OnDestroy, OnInit} from '@angular/core';
import {Subject, takeUntil} from 'rxjs';
import {MediaService} from "../../services/media-service/media.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AuthenticationService} from "../../services/authentication-service/authentication.service";
import {Provider} from "../../core/providers.enum";
import {ISignInRequest} from "../../services/authentication-service/types/sign-in.request";
import {ISignInResponse} from "../../services/authentication-service/types/sign-in.response";

@Component({
    selector: 'logout-page',
    templateUrl: './logout-page.component.html',
    styleUrls: ['./logout-page.component.scss'],
    standalone: false
})
export class LogoutPageComponent implements OnInit, OnDestroy {

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _authService: AuthenticationService;

    constructor(authService: AuthenticationService) {
        this._authService = authService;
    }

    public ngOnInit(): void {
        this._authService.logout()
            .pipe(takeUntil(this._destroy))
            .subscribe(() => {
                window.location.href = '/';
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
