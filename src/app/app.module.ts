import { NgModule, isDevMode } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomePageComponent } from '../pages/home-page/home-page.component';
import { WeatherService } from '../services/weather-service/weather.service';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { LinkService } from '../services/link-service/link.service';
import { UrlHealthCheckerComponent } from '../components/url-health-checker/url-health-checker.component';
import { LocationService } from '../services/location-service/location.service';
import { LinkRepository } from '../services/link-service/link.repository';
import { CustomLinkComponent } from '../components/custom-link/custom-link.component';
import { StatService } from '../services/stats-service/stat.service';
import { StatRepository } from '../services/stats-service/stat.repository';
import { PlexService } from '../services/plex-service/plex.service';
import { PlexRepository } from '../services/plex-service/plex.repository';
import { BuildService } from '../services/build-service/build.service';
import { BuildRepository } from '../services/build-service/build.repository';
import { WeatherComponent } from '../components/weather/weather.component';
import { AddLinkComponent } from '../components/add-link/add-link.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ResourceMonitorComponent } from '../components/resource-monitor/resource-monitor.component';
import { PlexDetailsComponent } from '../components/custom-link/custom-details/plex-details/plex-details.component';
import { ImportLinksComponent } from '../components/import-links/import-links.component';
import { ExportLinksComponent } from '../components/export-links/export-links.component';
import { WideButtonComponent } from '../components/wide-button/wide-button.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { FuelPriceService } from '../services/fuel-price-service/fuel-price.service';
import { FuelPriceRepository } from '../services/fuel-price-service/fuel-price.repository';
import { FuelPricesComponent } from '../components/fuel-prices/fuel-prices.component';
import { TimeagoModule } from 'ngx-timeago';
import { CustomMapComponent } from '../components/custom-map/custom-map.component';
import { FuelPricesPageComponent } from '../pages/fuel-prices-page/fuel-prices-page.component';
import { HeaderComponent } from '../components/header/header.component';
import { MenuComponent } from '../components/menu/menu.component';
import { UpdateDockerAppsPageComponent } from '../pages/update-docker-apps-page/update-docker-apps-page.component';
import { LinksComponent } from '../components/links/links.component';
import { DeployInfoComponent } from '../components/deploy-info/deploy-info.component';
import { TopInfoComponent } from '../components/top-info/top-info.component';
import { PiholeDetailsComponent } from '../components/custom-link/custom-details/pihole-details/pihole-details.component';
import { PiHoleService } from '../services/pihole-service/pi-hole.service';
import { PiHoleRepository } from '../services/pihole-service/pi-hole-repository.service';
import { RadarrDetailsComponent } from '../components/custom-link/custom-details/radarr-details/radarr-details.component';
import { RadarrService } from '../services/radarr-service/radarr.service';
import { RadarrRepository } from '../services/radarr-service/radarr.repository';
import { SonarrDetailsComponent } from '../components/custom-link/custom-details/sonarr-details/sonarr-details.component';
import { SonarrService } from '../services/sonarr-service/sonarr.service';
import { SonarrRepository } from '../services/sonarr-service/sonarr.repository';
import { ConfigsService } from '../services/configs-service/configs.service';
import { ConfigsRepository } from '../services/configs-service/configs.repository';

@NgModule({ declarations: [
        AppComponent,
        HomePageComponent,
        FuelPricesPageComponent,
        UrlHealthCheckerComponent,
        CustomLinkComponent,
        PlexDetailsComponent,
        WeatherComponent,
        AddLinkComponent,
        ResourceMonitorComponent,
        ImportLinksComponent,
        ExportLinksComponent,
        WideButtonComponent,
        FuelPricesComponent,
        CustomMapComponent,
        HeaderComponent,
        MenuComponent,
        UpdateDockerAppsPageComponent,
        LinksComponent,
        DeployInfoComponent,
        TopInfoComponent,
        PiholeDetailsComponent,
        RadarrDetailsComponent,
        SonarrDetailsComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        AppRoutingModule,
        ReactiveFormsModule,
        FormsModule,
        ServiceWorkerModule.register('ngsw-worker.js', {
            enabled: !isDevMode(),
            registrationStrategy: 'registerWhenStable:30000'
        }),
        TimeagoModule.forRoot()], providers: [
        WeatherService,
        LinkService,
        LocationService,
        LinkRepository,
        StatService,
        StatRepository,
        PlexService,
        PlexRepository,
        BuildService,
        BuildRepository,
        FuelPriceService,
        FuelPriceRepository,
        PiHoleService,
        PiHoleRepository,
        RadarrService,
        RadarrRepository,
        SonarrService,
        SonarrRepository,
        ConfigsService,
        ConfigsRepository,
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule {
}
