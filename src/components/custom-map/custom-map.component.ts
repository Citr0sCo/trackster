import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IFuelPrice } from '../../services/fuel-price-service/types/fuel-price.type';
import { ConfigsService } from '../../services/configs-service/configs.service';
import { IConfigs } from '../../services/configs-service/types/configs.type';

@Component({
    selector: 'custom-map',
    templateUrl: './custom-map.component.html',
    styleUrls: ['./custom-map.component.scss'],
    standalone: false
})
export class CustomMapComponent implements OnInit {

    @Input()
    public apiKey: string | null = null;

    @Input()
    public latitude: number | null = null;

    @Input()
    public longitude: number | null = null;

    @Input()
    public fuelStations: Array<IFuelPrice> = [];

    @Output()
    public clicked: EventEmitter<IFuelPrice> = new EventEmitter<IFuelPrice>();

    private readonly _configsService: ConfigsService;

    constructor(configsService: ConfigsService) {
        this._configsService = configsService;
    }

    public ngOnInit(): void {

        this._configsService.getAllConfigs()
            .subscribe((configs) => {
                // @ts-ignore
                mapboxgl.accessToken = configs.mapsApiKey;

                // @ts-ignore
                const map = new mapboxgl.Map({
                    container: 'map',
                    style: 'mapbox://styles/mapbox/streets-v12',
                    center: [this.longitude, this.latitude],
                    zoom: 11
                });

                // @ts-ignore
                const userLocation = new mapboxgl.Marker({ color: 'red' })
                    .setLngLat([this.longitude, this.latitude])
                    .addTo(map);

                userLocation.getElement().addEventListener('click', () => {
                    alert('Your location');
                });

                for (const fuelStation of this.fuelStations) {
                    // @ts-ignore
                    const fuelStationLocation = new mapboxgl.Marker({ color: fuelStation.colour })
                        .setLngLat([fuelStation.longitude, fuelStation.latitude])
                        .addTo(map);

                    fuelStationLocation.getElement().addEventListener('click', () => {
                        this.handlePinClick(fuelStation);
                    });
                }
            });
    }

    public handlePinClick(fuelPrice: IFuelPrice): void {
        this.clicked.emit(fuelPrice);
    }
}
