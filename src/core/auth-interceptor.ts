import {inject, Injectable} from "@angular/core";
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {Observable} from "rxjs";
import {AuthenticationService} from "../services/authentication-service/authentication.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const authToken = inject(AuthenticationService).getAuthToken();
        const newReq = req.clone({
            headers: req.headers.append('X-Authentication-Token', authToken ?? ''),
        });
        return next.handle(newReq);
    }
}