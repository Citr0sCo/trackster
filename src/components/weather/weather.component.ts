import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { WeatherService } from '../../services/weather-service/weather.service';
import { IWeatherData } from '../../services/weather-service/types/weather-data.type';
import { LocationService } from '../../services/location-service/location.service';

@Component({
    selector: 'weather',
    templateUrl: './weather.component.html',
    styleUrls: ['./weather.component.scss'],
    standalone: false
})
export class WeatherComponent implements OnInit, OnDestroy {

    @Input()
    public refreshCache: Subject<boolean> = new Subject<boolean>();

    public isCheckingWeather: boolean = false;
    public weather: IWeatherData | null = null;

    public currentTime: Date = new Date();

    private readonly _locationService: LocationService;
    private readonly _weatherService: WeatherService;
    private readonly _destroy: Subject<void> = new Subject();

    constructor(locationService: LocationService, weatherService: WeatherService) {
        this._locationService = locationService;
        this._weatherService = weatherService;
    }

    public ngOnInit() {
        this._locationService.getLocation()
            .pipe(takeUntil(this._destroy))
            .subscribe((response) => {
                this._weatherService.getWeatherFor(response.latitude, response.longitude)
                    .subscribe((response) => {
                        this.weather = response;
                    });
            });

        this.refreshCache
            .pipe(takeUntil(this._destroy))
            .subscribe((shouldRefresh) => {
                if (shouldRefresh) {
                    this.refreshWeather();
                }
            });
    }

    public getWeatherIcon(weatherDescription: string): string {

        if (weatherDescription.toUpperCase() === 'CLOUDS') {
            return 'fa fa-cloud';
        }

        if (weatherDescription.toUpperCase() === 'RAIN') {
            return 'fa fa-tint';
        }

        if (weatherDescription.toUpperCase() === 'CLEAR') {
            return 'fas fa-sun';
        }

        if (weatherDescription.toUpperCase() === 'SNOW') {
            return 'far fa-snowflake';
        }

        if (this.currentTime.getHours() < 6 && this.currentTime.getHours() > 18) {
            return 'fa fa-moon-o';
        }

        return 'fas fa-sun';
    }

    public roundWeatherTemperature(temperature: number): string {
        return `${Math.round(temperature * 100) / 100}℃`;
    }

    public titleCase(input: string): string {
        const splitStr = input.toLowerCase().split(' ');
        for (let i = 0; i < splitStr.length; i++) {
            splitStr[i] = splitStr[i].charAt(0).toUpperCase() + splitStr[i].substring(1);
        }
        return splitStr.join(' ');
    }

    public refreshWeather(): void {
        this.isCheckingWeather = true;

        this._locationService.getCurrentLocation()
            .pipe(takeUntil(this._destroy))
            .subscribe((location) => {
                this._weatherService.getLiveWeather(location.latitude, location.longitude)
                    .subscribe((response) => {
                        this.weather = response;
                        this.isCheckingWeather = false;
                    });
            });
    }

    public ngOnDestroy() {
        this._destroy.next();
    }
}
