import { Injectable, NgZone } from "@angular/core";
import { Observable } from "rxjs";
import { AuthenticationService } from '../services/authentication-service/authentication.service';

@Injectable({
    providedIn: 'root'
})
export class StreamService {

    private _authService: AuthenticationService;
    private _zone: NgZone;

    constructor(authService: AuthenticationService, zone: NgZone) {
        this._authService = authService;
        this._zone = zone;
    }

    public startStream(url: string, isDebug: boolean): Observable<string> {
        return new Observable<string>((observer) => {
            const eventSource = new EventSource(`${url}?token=${encodeURIComponent(this._authService.getAuthToken()!)}&isDebug=${isDebug ? 'true' : 'false'}`);

            // EventSource callbacks run outside the Angular zone, so streamed updates
            // only reached the view on the next unrelated change detection pass.
            eventSource.onmessage = event => this._zone.run(() => observer.next(event.data));

            eventSource.onerror = error => {
                eventSource.close();
                this._zone.run(() => observer.error(error));
            };

            return () => eventSource.close();
        });
    }
}
