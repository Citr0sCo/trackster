import { Component, OnDestroy, OnInit } from '@angular/core';
import {Subject, takeUntil} from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import {ActivatedRoute, Router} from "@angular/router";
import {AuthenticationService} from "../../services/authentication-service/authentication.service";
import {Provider} from "../../core/providers.enum";

@Component({
    selector: 'authorize-trakt',
    templateUrl: './authorize-trakt.component.html',
    styleUrls: ['./authorize-trakt.component.scss'],
    standalone: false
})
export class AuthorizeTraktComponent implements OnInit, OnDestroy {

    private readonly _destroy: Subject<void> = new Subject();
    private _router: ActivatedRoute;
    private readonly _authenticationService: AuthenticationService;

    constructor(router: ActivatedRoute, authenticationService: AuthenticationService) {
        this._router = router;
        this._authenticationService = authenticationService;
    }

    public ngOnInit(): void {
        this._router.queryParams
            .pipe(takeUntil(this._destroy))
            .subscribe((params: any) => {

                this._authenticationService.signIn(Provider.Trakt, {
                    code: params.code
                })
                .pipe(takeUntil(this._destroy))
                .subscribe((response) => {
                    console.log(response);
                })
            });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
