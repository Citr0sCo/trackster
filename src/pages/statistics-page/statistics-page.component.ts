import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { MovieService } from '../../services/movie-service/movie.service';
import { IWatchedMovie } from '../../services/movie-service/types/watched-movie.type';
import { ShowService } from '../../services/show-service/show.service';
import { IWatchedEpisode } from '../../services/show-service/types/watched-episode.type';
import { UserService } from '../../services/user-service/user.service';

interface IChartPoint {
    label: string;
    value: number;
    x: number;
    y: number;
}

interface ILineChart {
    points: IChartPoint[];
    linePath: string;
    areaPath: string;
    maxValue: number;
}

interface IGenreSlice {
    name: string;
    value: number;
    percentage: number;
    color: string;
}

@Component({
    selector: 'statistics-page',
    templateUrl: './statistics-page.component.html',
    styleUrls: ['./statistics-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class StatisticsPageComponent implements OnInit, OnDestroy {

    public statsLoading: boolean = false;
    public calendarStatsLoading: boolean = false;
    public totalWatched: number = 0;
    public totalMovies: number = 0;
    public totalEpisodes: number = 0;
    public calendarItems: { key: string; value: number }[] = [];
    public calendarItemsMonths: Date[] = [];
    public startDate: Date = new Date();
    public endDate: Date = new Date();
    public calendarMaxValue: number = 0;
    public genreSlices: IGenreSlice[] = [];
    public timeOfDayChart: ILineChart = this.emptyLineChart();
    public weekdayChart: ILineChart = this.emptyLineChart();
    public readonly timeOfDayLabels: string[] = ['12a', '3a', '6a', '9a', '12p', '3p', '6p', '9p'];
    public readonly weekdayLabels: string[] = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _mediaService: MediaService;
    private readonly _movieService: MovieService;
    private readonly _showService: ShowService;
    private readonly _userService: UserService;
    private history: Map<string, number> = new Map<string, number>();

    constructor(mediaService: MediaService, movieService: MovieService, showService: ShowService, userService: UserService) {
        this._mediaService = mediaService;
        this._movieService = movieService;
        this._showService = showService;
        this._userService = userService;
    }

    public ngOnInit(): void {
        this.loadStatistics();
    }

    private loadStatistics(): void {
        this.statsLoading = true;
        this.calendarStatsLoading = true;

        this._userService.getUserBySession()
            .pipe(takeUntil(this._destroy))
            .subscribe((user) => {
                forkJoin({
                    stats: this._mediaService.getStats(user.username),
                    calendar: this._mediaService.getStatsForCalendar(user.username, 400),
                    movies: this._movieService.getAllMoviesFor(user.username),
                    shows: this._showService.getAllShowsFor(user.username),
                })
                    .pipe(takeUntil(this._destroy))
                    .subscribe(({ stats, calendar, movies, shows }) => {
                        this.statsLoading = false;
                        this.calendarStatsLoading = false;
                        this.totalWatched = stats.totalWatched;
                        this.totalMovies = stats.totalMoviesWatched;
                        this.totalEpisodes = stats.totalEpisodesWatched;
                        this.history = new Map(Object.entries(calendar));
                        this.calendarItems = [];
                        this.calendarItemsMonths = [];
                        this.calendarMaxValue = 0;
                        this.generateCalendarMatrix();
                        this.parseMediaForHistory();
                        this.buildAnalytics(movies, shows);
                    });
            });
    }

    private buildAnalytics(movies: IWatchedMovie[], shows: IWatchedEpisode[]): void {
        const now = new Date();
        const yearAgo = new Date(now);
        yearAgo.setFullYear(now.getFullYear() - 1);
        const genreCounts = new Map<string, number>();
        const timeCounts = new Array<number>(24).fill(0);
        const weekdayCounts = new Array<number>(7).fill(0);
        const watchedEvents = [
            ...movies.map((entry) => ({
                watchedAt: new Date(entry.watchedAt),
                genres: entry.movie.genres,
            })),
            ...shows.map((entry) => ({
                watchedAt: new Date(entry.watchedAt),
                genres: entry.episode.season.show.genres,
            })),
        ];

        for (const event of watchedEvents) {
            for (const genre of event.genres || []) {
                genreCounts.set(genre.name, (genreCounts.get(genre.name) || 0) + 1);
            }

            if (event.watchedAt >= yearAgo && event.watchedAt <= now) {
                timeCounts[event.watchedAt.getHours()]++;
                weekdayCounts[(event.watchedAt.getDay() + 6) % 7]++;
            }
        }

        this.genreSlices = this.createGenreSlices(genreCounts);
        this.timeOfDayChart = this.createLineChart(timeCounts, this.timeOfDayLabels, 3);
        this.weekdayChart = this.createLineChart(weekdayCounts, this.weekdayLabels, 1);
    }

    private createGenreSlices(counts: Map<string, number>): IGenreSlice[] {
        const colors = ['#9774f5', '#a7e0dc', '#f1c58c', '#ed7892', '#d48cf5', '#b8a4ff'];
        const entries = [...counts.entries()].sort((a, b) => b[1] - a[1]);
        const visibleEntries = entries.slice(0, 5);
        const remainder = entries.slice(5).reduce((total, [, value]) => total + value, 0);
        if (remainder > 0) {
            visibleEntries.push(['Other', remainder]);
        }

        const total = visibleEntries.reduce((sum, [, value]) => sum + value, 0);
        return visibleEntries.map(([name, value], index) => {
            const percentage = total ? (value / total) * 100 : 0;
            return { name, value, percentage, color: colors[index % colors.length] };
        });
    }

    public get genrePieStyle(): string {
        let offset = 0;
        const stops = this.genreSlices.map((slice) => {
            const start = offset;
            offset += slice.percentage;
            return `${slice.color} ${start}% ${offset}%`;
        });
        return `conic-gradient(${stops.join(', ') || 'rgba(255,255,255,.08) 0 100%'})`;
    }

    private createLineChart(values: number[], labels: string[], labelStep: number): ILineChart {
        const left = 38;
        const right = 700;
        const top = 18;
        const bottom = 190;
        const maxValue = Math.max(...values, 1);
        const points = values.map((value, index) => ({
            label: index % labelStep === 0 ? labels[index / labelStep] || '' : '',
            value,
            x: left + (index / Math.max(values.length - 1, 1)) * (right - left),
            y: bottom - (value / maxValue) * (bottom - top),
        }));
        const linePath = points.map((point, index) => `${index === 0 ? 'M' : 'L'} ${point.x.toFixed(1)} ${point.y.toFixed(1)}`).join(' ');
        const areaPath = `${linePath} L ${right} ${bottom} L ${left} ${bottom} Z`;
        return { points, linePath, areaPath, maxValue };
    }

    private emptyLineChart(): ILineChart {
        return this.createLineChart([0, 0], ['', ''], 1);
    }

    public formatChartValue(value: number): string {
        return value.toString();
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
        this._movieService.bustCache();
        this._showService.bustCache();
        this.loadStatistics();
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
