import { NgModule, isDevMode } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomePageComponent } from '../pages/home-page/home-page.component';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { MediaService } from '../services/media-service/media.service';
import { MediaRepository } from '../services/media-service/media.repository';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { WideButtonComponent } from '../components/wide-button/wide-button.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { TimeagoModule } from 'ngx-timeago';
import { AuthorizeTraktComponent } from "../pages/authorize-trakt-page/authorize-trakt.component";
import { AuthenticationService } from "../services/authentication-service/authentication.service";
import { AuthenticationRepository } from "../services/authentication-service/authentication.repository";
import { HistoryPageComponent } from '../pages/history-page/history-page.component';
import { CustomSidebarComponent } from '../components/custom-sidebar/custom-sidebar.component';
import { EventService } from "../services/event-service/event.service";
import { MediaPosterComponent } from "../components/media-poster/media-poster.component";
import { SettingsPageComponent } from "../pages/settings-page/settings-page.component";
import { MediaDetailsPageComponent } from "../pages/media-details-page/media-details-page.component";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
    declarations: [
        AppComponent,
        HomePageComponent,
        HistoryPageComponent,
        SettingsPageComponent,
        AuthorizeTraktComponent,
        WideButtonComponent,
        CustomSidebarComponent,
        MediaPosterComponent,
        MediaDetailsPageComponent
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        ReactiveFormsModule,
        FormsModule,
        ServiceWorkerModule.register('ngsw-worker.js', {
            enabled: !isDevMode(),
            registrationStrategy: 'registerWhenStable:30000'
        }),
        TimeagoModule.forRoot(),
        BrowserAnimationsModule
    ], providers: [
        MediaService,
        MediaRepository,
        AuthenticationService,
        AuthenticationRepository,
        EventService,
        provideHttpClient(withInterceptorsFromDi())
    ]
})
export class AppModule {
}
