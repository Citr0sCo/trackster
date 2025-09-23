import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { AuthenticationService } from '../services/authentication-service/authentication.service';

@Injectable({
    providedIn: 'root'
})
export class StreamService {

    private eventSource: EventSource | null = null;
    private _authService: AuthenticationService;

    constructor(authService: AuthenticationService) {
        this._authService = authService;
    }

    public startStream(url: string, isDebug: boolean): Observable<any> {
        return new Observable((observer) => {
            this.eventSource = new EventSource(`${url}?token=${encodeURIComponent(this._authService.getAuthToken()!)}&isDebug=${isDebug ? 'true' : 'false'}`);

            this.eventSource.onmessage = event => {
                observer.next(event.data);
            };

            this.eventSource.onerror = error => {
                observer.error(error);
                this.eventSource!.close();
            };

            return () => {
                this.eventSource!.close();
            };
        });
    }
}