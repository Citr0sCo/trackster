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
    public seasonNumber: number = 0;
    public episodeNumber: number = 0;
    public isPaused: boolean = false;
    public isVisible: boolean = false;
    public isDesktop: boolean = false;
    public type: string | null = null;
    public slug: string | null = null;

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

            this.type = 'Movie';

            if (payload.Response.Data.Action === "Start") {
                this.isVisible = true;
                this.isPaused = false;

                this.title = payload.Response.Data.Movie.Title;
                this.slug = payload.Response.Data.Movie.Slug;
                this.year = payload.Response.Data.Movie.Year;
                this.progress = payload.Response.Data.MillisecondsWatched;
                this.duration = payload.Response.Data.Duration;
            }

            if (payload.Response.Data.Action === "Paused") {
                this.isVisible = true;
                this.isPaused = true;

                this.title = payload.Response.Data.Movie.Title;
                this.slug = payload.Response.Data.Movie.Slug;
                this.year = payload.Response.Data.Movie.Year;
                this.progress = payload.Response.Data.MillisecondsWatched;
                this.duration = payload.Response.Data.Duration;
            }

            if (payload.Response.Data.Action === "Stop") {
                this.isVisible = false;
                this.isPaused = false;
            }
        });

        this._webSocketService.subscribe(WebSocketKey.WatchingNowEpisode, (payload) => {

            this.type = 'Episode';

            if (payload.Response.Data.Action === "Start") {
                this.isVisible = true;
                this.isPaused = false;

                this.title = payload.Response.Data.Episode.Title;
                this.parentTitle = payload.Response.Data.Episode.Season.Title;
                this.grandParentTitle = payload.Response.Data.Episode.Season.Show.Title;
                this.slug = payload.Response.Data.Episode.Season.Show.Slug;
                this.seasonNumber = payload.Response.Data.Episode.Season.Number;
                this.episodeNumber = payload.Response.Data.Episode.Number;
                this.year = payload.Response.Data.Episode.Season.Show.Year;
                this.progress = payload.Response.Data.MillisecondsWatched;
                this.duration = payload.Response.Data.Duration;
            }

            if (payload.Response.Data.Action === "Paused") {
                this.isVisible = true;
                this.isPaused = true;

                this.title = payload.Response.Data.Episode.Title;
                this.parentTitle = payload.Response.Data.Episode.Season.Title;
                this.grandParentTitle = payload.Response.Data.Episode.Season.Show.Title;
                this.slug = payload.Response.Data.Episode.Season.Show.Slug;
                this.year = payload.Response.Data.Episode.Season.Show.Year;
                this.progress = payload.Response.Data.MillisecondsWatched;
                this.duration = payload.Response.Data.Duration;
            }

            if (payload.Response.Data.Action === "Stop") {
                this.isVisible = false;
                this.isPaused = false;
            }
        });


        setInterval(() => {
            if (!this.isPaused && this.isVisible) {
                this.progress += 1000;
            }
        }, 1000);
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
