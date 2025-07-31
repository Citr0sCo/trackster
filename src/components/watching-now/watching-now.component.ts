import { Component, OnInit } from '@angular/core';
import { WebSocketKey } from '../../services/websocket-service/types/web-socket.key';
import { WebSocketService } from '../../services/websocket-service/web-socket.service';

@Component({
    selector: 'watching-now',
    templateUrl: './watching-now.component.html',
    styleUrls: ['./watching-now.component.scss'],
    standalone: false,
})
export class WatchingNowComponent implements OnInit {

    public title: string = '';
    public parentTitle: string = '';
    public grandParentTitle: string = '';
    public year: number = 0;
    public progress: number = 0;
    public duration: number = 0;
    public isVisible: boolean = false;
    public isDesktop: boolean = false;

    private _webSocketService: WebSocketService;

    constructor() {
        this._webSocketService = WebSocketService.instance();
    }

    public ngOnInit(): void {

        if (window.innerWidth > 768) {
            this.isDesktop = true;
        } else {
            this.isDesktop = false;
        }

        window.addEventListener('resize', () => {
            if (window.innerWidth > 768) {
                this.isDesktop = true;
            } else {
                this.isDesktop = false;
            }
        });

        this._webSocketService.subscribe(WebSocketKey.WatchingNowMovie, (payload) => {
            if (payload.Response.Data.Action === "Start") {
                this.isVisible = true;

                this.title = payload.Response.Data.Movie.Title;
                this.year = payload.Response.Data.Movie.Year;
                this.progress = payload.Response.Data.MillisecondsWatched;
                this.duration = payload.Response.Data.Duration;
            }
            if (payload.Response.Data.Action === "Stop") {
                this.isVisible = false;
            }
        });

        this._webSocketService.subscribe(WebSocketKey.WatchingNowEpisode, (payload) => {

            if (payload.Response.Data.Action === "Start") {
                this.isVisible = true;

                this.title = payload.Response.Data.Episode.Title;
                this.parentTitle = payload.Response.Data.Episode.Season.Title;
                this.grandParentTitle = payload.Response.Data.Episode.Season.Show.Title;
                this.year = payload.Response.Data.Episode.Season.Show.Year;
                this.progress = payload.Response.Data.MillisecondsWatched;
                this.duration = payload.Response.Data.Duration;
            }

            if (payload.Response.Data.Action === "Stop") {
                this.isVisible = false;
            }
        });


        setInterval(() => {
            this.progress += 1000;
        }, 1000 * 60);
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

    public getPercentage(progress: number, total: number): number {
        return Math.round((progress / total) * 100);
    }

    public trimTitle(title: string): string {

        if (this.isDesktop) {
            return title;
        }

        if (title.length === 0) {
            return '';
        }

        const maxLength = 15;

        if (title.length > maxLength) {
            return `${title.substring(0, maxLength).trim()}...`;
        }

        return title;
    }
}
