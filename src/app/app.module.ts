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

@NgModule({
    declarations: [
        AppComponent,
        HomePageComponent,
        WideButtonComponent,
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
        MediaService,
        MediaRepository,
        provideHttpClient(withInterceptorsFromDi())
    ]
})
export class AppModule {
}
