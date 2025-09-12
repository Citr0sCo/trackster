import {AfterViewInit, Component, OnDestroy, OnInit} from '@angular/core';
import {EventService} from "../../services/event-service/event.service";
import {WebSocketService} from "../../services/websocket-service/web-socket.service";
import {WebSocketKey} from "../../services/websocket-service/types/web-socket.key";
import {UserService} from "../../services/user-service/user.service";
import {Subject, takeUntil} from "rxjs";

@Component({
    selector: 'login-page',
    templateUrl: './dashboard-page.component.html',
    styleUrls: ['./dashboard-page.component.scss'],
    standalone: false
})
export class DashboardPageComponent implements AfterViewInit, OnInit, OnDestroy {

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _eventService: EventService;
    private readonly  _webSocketService: WebSocketService;
    private readonly  _userService: UserService;

    constructor(eventService: EventService, userService: UserService) {
        this._eventService = eventService;
        this._userService = userService;
        this._webSocketService = WebSocketService.instance();
    }

    public ngOnInit(): void {

        this._userService.getUserBySession()
            .pipe(takeUntil(this._destroy))
            .subscribe((user) => {
                this._webSocketService.send(WebSocketKey.Handshake, { Test: 'Hello World!', UserReference: user.identifier });
            });
    }

    public ngAfterViewInit(): void {

        const element = document.querySelector('.main-content');
        element!.addEventListener('scroll', () => {
            if (element!.scrollHeight - element!.clientHeight <= element!.scrollTop + 10) {
                this._eventService.scrolledToBottomOfThePage();
            } else {
                this._eventService.notScrolledToBottomOfThePage();
            }
        });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
