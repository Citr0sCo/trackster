import {Observable} from "rxjs";
import {Injectable} from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class StreamService {

    private eventSource: EventSource | null = null;

    public startStream(url: string): Observable<any> {
        return new Observable((observer) => {
            this.eventSource = new EventSource(url);

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