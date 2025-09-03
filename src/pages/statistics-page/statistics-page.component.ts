import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { IMedia, MediaType } from '../../services/media-service/types/media.type';

@Component({
    selector: 'statistics-page',
    templateUrl: './statistics-page.component.html',
    styleUrls: ['./statistics-page.component.scss'],
    standalone: false
})
export class StatisticsPageComponent implements OnInit, OnDestroy {

    public isImporting: boolean = false;
    public username: string = 'citr0s';
    public media: Array<IMedia> = [];
    public totalMovies: number = 0;
    public totalEpisodes: number = 0;
    public calendarItems: { key: string; value: number }[] = [];
    public startDate: Date = new Date();
    public endDate: Date = new Date();
    public calendarMaxValue: number = 0;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;
    private history: Map<string, number> = new Map<string, number>();

    constructor(mediaService: MediaService) {
        this._mediaService = mediaService;
    }

    public ngOnInit(): void {
        this.isImporting = true;

        this._mediaService.getHistoryForUser('citr0s')
            .pipe(takeUntil(this._destroy))
            .subscribe((media) => {
                this.isImporting = false;

                this.media = media;
                this.totalMovies = this.media.filter((x) => x.mediaType === MediaType.Movie).length;
                this.totalEpisodes = this.media.filter((x) => x.mediaType === MediaType.Episode).length;

                this.parseMediaForHistory(media);
            });
    }

    private parseMediaForHistory(media: Array<IMedia>) {
        for (let item of media) {

            let day = item.watchedAt.getDate();
            let month = item.watchedAt.getMonth();

            let parsedDay = day.toString();
            if (day < 10) {
                parsedDay = `0${day}`;
            }

            let parsedMonth = month.toString();
            if (month < 10) {
                parsedMonth = `0${month}`;
            }

            const key = `${item.watchedAt.getFullYear()}-${parsedMonth}-${parsedDay}`;

            if (this.history.has(key)) {
                const currentValue = this.history.get(key)!;
                const newValue = currentValue + 1;
                this.history.set(key, newValue);
            } else {
                this.history.set(key, 1);
            }
        }

        const endDate = this.endDate;
        const startDate = new Date(endDate);
        startDate.setDate(endDate.getDate() - 365);
        this.startDate = startDate;

        let years = [endDate.getFullYear()];
        if (endDate.getFullYear() !== startDate.getFullYear()) {
            years = [startDate.getFullYear(), endDate.getFullYear()];
        }

        console.log(years);

        for (let year of years) {
            for (let month = 0; month < 12; month++) {

                const actualMonth = 1 + month;
                const days = this.daysInMonth(year, actualMonth);

                for (let day = 1; day <= days; day++) {

                    let parsedDay = day.toString();
                    if (day < 10) {
                        parsedDay = `0${day}`;
                    }

                    let parsedMonth = actualMonth.toString();
                    if (actualMonth < 10) {
                        parsedMonth = `0${actualMonth}`;
                    }

                    const key = `${year}-${parsedMonth}-${parsedDay}`;
                    const parsedDate = new Date(key);

                    if (parsedDate < startDate) {
                        continue;
                    }

                    if (parsedDate > endDate) {
                        continue;
                    }

                    this.calendarItems.push({ key: key, value: 0 });
                }
            }
        }

        for (let entry of this.history.entries()) {
            this.calendarItems = this.calendarItems.map((item) => {

                if (item.key == entry[0]) {
                    item.value = entry[1];
                }

                return item;
            });
        }
    }

    private daysInMonth(year: number, month: number): number {
        return new Date(year, month, 0).getDate();
    }

    public generateCalenderItemColour(value: number): string {

        let highestNumber = this.calendarMaxValue;

        for (let entry of this.calendarItems) {
            if (entry.value > highestNumber) {
                highestNumber = entry.value;
            }
        }

        this.calendarMaxValue = highestNumber;

        return (Math.floor((value / highestNumber) * 100) / 100).toString();
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
