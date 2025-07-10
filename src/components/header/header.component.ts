import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { WebSocketService } from '../../services/websocket-service/web-socket.service';
import { WebSocketKey } from '../../services/websocket-service/types/web-socket.key';

@Component({
    selector: 'header-component',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
    standalone: false
})
export class HeaderComponent implements OnInit, OnDestroy {

    public currentTime: Date = new Date();
    public webQuery: string = '';
    public isConnected: boolean = false;

    private readonly _subscriptions: Subscription = new Subscription();
    private readonly _webSocketService: WebSocketService;

    constructor() {
        this._webSocketService = WebSocketService.instance();
    }

    public ngOnInit(): void {
        this._subscriptions.add(
            this._webSocketService.isConnected
                .asObservable()
                .subscribe((isConnected: boolean) => {
                    this.isConnected = isConnected;

                    if (!this.isConnected) {
                        console.log('Attempting to reconnect to websocket in 5 seconds...');
                        setTimeout(() => {
                            if (location.href.indexOf('https') > -1 || location.href.indexOf('localhost') > -1) {
                                this._webSocketService.connect(true);
                            } else {
                                this._webSocketService.connect();
                            }
                        }, 5000);
                    }
                })
        );

        setInterval(() => {
            this.currentTime = new Date();
        }, 1000);

        this._webSocketService.send(WebSocketKey.Handshake, { Test: 'Hello World!' });
    }

    public getGreeting(): string {

        let greeting = 'Welcome';

        if (this.currentTime.getHours() < 12) {
            greeting = 'Good Morning';
        }
        if (this.currentTime.getHours() > 12) {
            greeting = 'Good Afternoon';
        }
        if (this.currentTime.getHours() > 18) {
            greeting = 'Good Evening';
        }

        return greeting;
    }

    public searchWeb(): void {
        window.location.href = `https://www.google.com/search?q=${this.webQuery}`;
    }

    public ngOnDestroy(): void {
        this._subscriptions.unsubscribe();
    }
}
