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
            console.log('Received Data Watching Now Movie', payload);
        });

        this._webSocketService.subscribe(WebSocketKey.WatchingNowEpisode, (payload) => {
            console.log('Received Data Watching Now Episode', payload);
        });
    }
}
