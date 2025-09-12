import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { IMedia, MediaType } from '../../services/media-service/types/media.type';
import { UserService } from '../../services/user-service/user.service';

@Component({
    selector: 'statistics-page',
    templateUrl: './statistics-page.component.html',
    styleUrls: ['./statistics-page.component.scss'],
    standalone: false
})
export class StatisticsPageComponent implements OnInit, OnDestroy {

    public statsLoading: boolean = false;
    public calendarStatsLoading: boolean = false;
    public username: string = 'citr0s';
    public media: Array<IMedia> = [];
    public totalWatched: number = 0;
    public totalMovies: number = 0;
    public totalEpisodes: number = 0;
    public calendarItems: { key: string; value: number }[] = [];
    public calendarItemsMonths: Date[] = [];
    public startDate: Date = new Date();
    public endDate: Date = new Date();
    public calendarMaxValue: number = 0;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;
    private readonly _userService: UserService;
    private history: Map<string, number> = new Map<string, number>();

    constructor(mediaService: MediaService, userService: UserService) {
        this._mediaService = mediaService;
        this._userService = userService;
    }

    public ngOnInit(): void {
        this.statsLoading = true;
        this.calendarStatsLoading = true;

        this._userService.getUserBySession()
            .pipe(takeUntil(this._destroy))
            .subscribe((user) => {
                this._mediaService.getStats(user.username)
                    .pipe(takeUntil(this._destroy))
                    .subscribe((stats) => {
                        this.statsLoading = false;

                        this.totalWatched = stats.totalWatched;
                        this.totalMovies = stats.totalMoviesWatched;
                        this.totalEpisodes = stats.totalEpisodesWatched;
                    });

                this._mediaService.getStatsForCalendar(user.username, 400)
                    .pipe(takeUntil(this._destroy))
                    .subscribe((stats) => {
                        this.calendarStatsLoading = false;
                        this.calendarItems = [];
                        this.history = new Map<string, number>();

                        for (let entry of Object.entries(stats)) {
                            this.history.set(entry[0], entry[1]);
                        }

                        this.generateCalendarMatrix();
                        this.parseMediaForHistory();
                    });
            });
    }

    private generateCalendarMatrix(): void {
        const endDate = this.endDate;
        const startDate = new Date(endDate);
        startDate.setDate(endDate.getDate() - 365);

        while (startDate.getDay() !== 1) {
            startDate.setDate(startDate.getDate() - 1);
        }

        this.startDate = startDate;

        let years = [endDate.getFullYear()];
        if (endDate.getFullYear() !== startDate.getFullYear()) {
            years = [startDate.getFullYear(), endDate.getFullYear()];
        }

        for (let year of years) {
            for (let month = 0; month < 12; month++) {

                const actualMonth = 1 + month;
                const days = this.daysInMonth(year, actualMonth);

                for (let day = 0; day < days; day++) {

                    const actualDay = 1 + day;

                    let parsedDay = actualDay.toString();
                    if (actualDay < 10) {
                        parsedDay = `0${actualDay}`;
                    }

                    let parsedMonth = actualMonth.toString();
                    if (actualMonth < 10) {
                        parsedMonth = `0${actualMonth}`;
                    }

                    const key = `${year}-${parsedMonth}-${parsedDay}`;
                    const parsedDate = new Date(key);

                    if (parsedDate.getTime() < startDate.getTime()) {
                        continue;
                    }

                    if (parsedDate.getTime() > endDate.getTime()) {
                        continue;
                    }

                    if (this.calendarItems.length === 0) {
                        if (parsedDate.getDay() !== 1) {
                            continue;
                        }
                    }

                    this.calendarItems.push({ key: key, value: 0 });
                }
            }
        }
    }

    private parseMediaForHistory() {
        for (let entry of this.history.entries()) {
            this.calendarItems = this.calendarItems.map((item) => {

                if (item.key == entry[0]) {
                    item.value = entry[1];
                }

                if (item.value > this.calendarMaxValue) {
                    this.calendarMaxValue = item.value;
                }

                const date = new Date(item.key);
                const parsedDate = new Date(`${date.getFullYear()}-${date.getMonth() + 1}-01`);

                const alreadyAdded = this.calendarItemsMonths.find((x) => x.toDateString() === parsedDate.toDateString());

                if (!alreadyAdded) {
                    this.calendarItemsMonths.push(parsedDate);
                }

                return item;
            });
        }
    }

    private daysInMonth(year: number, month: number): number {
        return new Date(year, month, 0).getDate();
    }

    public generateCalenderItemColour(value: number): string {
        return (Math.floor((value / this.calendarMaxValue) * 100) / 100).toString();
    }

    public bustCache(): void {
        this._mediaService.bustCache();
        this.ngOnInit();
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
