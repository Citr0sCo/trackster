import { NgModule, isDevMode } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomePageComponent } from '../pages/home-page/home-page.component';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
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
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { WatchingNowComponent } from '../components/watching-now/watching-now.component';
import { ShowDetailsPageComponent } from "../pages/show-details-page/show-details-page.component";
import { MovieDetailsPageComponent } from "../pages/movie-details-page/movie-details-page.component";
import { StatisticsPageComponent } from '../pages/statistics-page/statistics-page.component';
import { StreamService } from "../core/event-service.service";
import { AuthInterceptor } from "../core/auth-interceptor";
import { DashboardPageComponent } from "../pages/dashboard-page/dashboard-page.component";
import { LoginPageComponent } from "../pages/login-page/login-page.component";
import { RegisterPageComponent } from "../pages/register-page/register-page.component";
import { LogoutPageComponent } from "../pages/logout-page/logout-page.component";
import { UserRepository } from '../services/user-service/user.repository';
import { UserService } from '../services/user-service/user.service';
import { WebhookService } from "../services/webhook-service/webhook.service";
import { WebhookRepository } from "../services/webhook-service/webhook.repository";
import { MovieService } from "../services/movie-service/movie.service";
import { MovieRepository } from "../services/movie-service/movie.repository";
import { ShowService } from "../services/show-service/show.service";
import { ShowRepository } from "../services/show-service/show.repository";
import { SettingsService } from '../services/settings-service/settings.service';
import { SettingsRepository } from '../services/settings-service/settings.repository';
import { AuthWithTraktButtonComponent } from '../components/auth-with-trakt-buton/auth-with-trakt-button.component';

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
        WatchingNowComponent,
        ShowDetailsPageComponent,
        MovieDetailsPageComponent,
        StatisticsPageComponent,
        DashboardPageComponent,
        LoginPageComponent,
        RegisterPageComponent,
        LogoutPageComponent,
        AuthWithTraktButtonComponent
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
        StreamService,
        UserService,
        UserRepository,
        WebhookService,
        WebhookRepository,
        MovieService,
        MovieRepository,
        ShowService,
        ShowRepository,
        SettingsService,
        SettingsRepository,
        provideHttpClient(withInterceptorsFromDi()),
        {
            provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true
        }
    ]
})
export class AppModule {
}
