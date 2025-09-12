import {inject, Injectable} from "@angular/core";
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {catchError, EMPTY, Observable} from "rxjs";
import {AuthenticationService} from "../services/authentication-service/authentication.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    private _authService: AuthenticationService;

    constructor(authService: AuthenticationService) {
        this._authService = authService;
    }

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const authToken = inject(AuthenticationService).getAuthToken();
        const newReq = req.clone({
            headers: req.headers.append('X-Authentication-Token', authToken ?? ''),
        });
        return next.handle(newReq)
            .pipe(catchError((error) => {
                if (error.exception.status === 401) {
                    this._authService.logout()
                        .subscribe(() => {
                            window.location.href = "/#/login";
                        });
                }
                return EMPTY;
            }),);
    }
}