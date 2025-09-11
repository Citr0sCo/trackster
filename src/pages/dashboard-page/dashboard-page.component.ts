import {AfterViewInit, Component} from '@angular/core';
import {EventService} from "../../services/event-service/event.service";
import {WebSocketService} from "../../services/websocket-service/web-socket.service";
import {WebSocketKey} from "../../services/websocket-service/types/web-socket.key";

@Component({
    selector: 'login-page',
    templateUrl: './dashboard-page.component.html',
    styleUrls: ['./dashboard-page.component.scss'],
    standalone: false
})
export class DashboardPageComponent implements AfterViewInit {

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
    }
}
