import { AfterViewInit, Component } from '@angular/core';
import { EventService } from "../services/event-service/event.service";
import { WebSocketService } from '../services/websocket-service/web-socket.service';
import { WebSocketKey } from '../services/websocket-service/types/web-socket.key';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
})
export class AppComponent implements AfterViewInit {

    private _eventService: EventService;
    private _webSocketService: WebSocketService;

    constructor(eventService: EventService) {
        this._eventService = eventService;
        this._webSocketService = WebSocketService.instance();
    }

    public ngAfterViewInit(): void {

        this._webSocketService.send(WebSocketKey.Handshake, { Test: 'Hello World!' });

        const element = document.querySelector('.main-content');
        element!.addEventListener('scroll', () => {
            if (element!.scrollHeight - element!.clientHeight <= element!.scrollTop + 10) {
                this._eventService.scrolledToBottomOfThePage();
            } else {
                this._eventService.notScrolledToBottomOfThePage();
            }
        });

        this._webSocketService.subscribe(WebSocketKey.WatchingNowMovie, (payload) => {
            if (payload.Response.Data.Action === "Start") {
                console.log(`Started watching: ${payload.Response.Data.Movie.Title} @ ${payload.Response.Data.StartedAt} ${payload.Response.Data.MillisecondsWatched}ms into it.`);
            }
            if (payload.Response.Data.Action === "Stop") {
                console.log(`Stopped watching movie.`);
            }
        });

        this._webSocketService.subscribe(WebSocketKey.WatchingNowEpisode, (payload) => {
            if (payload.Response.Data.Action === "Start") {
                console.log(`Started watching: ${payload.Response.Data.Episode.Title} @ ${payload.Response.Data.StartedAt} ${payload.Response.Data.MillisecondsWatched}ms into it.`);
            }
            if (payload.Response.Data.Action === "Stop") {
                console.log(`Stopped watching episode.`);
            }
        });
    }
}
