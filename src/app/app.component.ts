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
                console.log(`Started watching: ${payload.Response.Data.Movie.Title} - ${this.getTimeFromDuration(payload.Response.Data.Duration)} - ${this.getTimeFromDuration(payload.Response.Data.MillisecondsWatched)}`);
            }
            if (payload.Response.Data.Action === "Stop") {
                console.log(`Stopped watching movie.`);
            }
        });

        this._webSocketService.subscribe(WebSocketKey.WatchingNowEpisode, (payload) => {
            if (payload.Response.Data.Action === "Start") {
                console.log(`Started watching: ${payload.Response.Data.Episode.Season.Show.Title} - ${this.getTimeFromDuration(payload.Response.Data.Duration)} - ${this.getTimeFromDuration(payload.Response.Data.MillisecondsWatched)}`);
            }
            if (payload.Response.Data.Action === "Stop") {
                console.log(`Stopped watching episode.`);
            }
        });
    }

    public getTimeFromDuration(duration: number): string {

        const date = new Date(duration);

        let displayText = '';

        const hours = date.getUTCHours();
        if (hours > 0) {
            displayText += `${hours}:`;
        }

        const minutes = date.getMinutes();
        if (minutes < 10) {
            displayText += `0${minutes}:`;
        } else {
            displayText += `${minutes}:`;
        }

        const seconds = date.getSeconds();
        if (seconds < 10) {
            displayText += `0${seconds}`;
        } else {
            displayText += `${seconds}`;
        }

        return displayText;
    }
}
