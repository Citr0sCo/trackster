import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from '../pages/home-page/home-page.component';
import {AuthorizeTraktComponent} from "../pages/authorize-trakt-page/authorize-trakt.component";
import { HistoryPageComponent } from '../pages/history-page/history-page.component';
import {SettingsPageComponent} from "../pages/settings-page/settings-page.component";
import {MediaDetailsPageComponent} from "../pages/media-details-page/media-details-page.component";

const routes: Routes = [
    {
        path: 'home',
        component: HomePageComponent
    },
    {
        path: 'history',
        component: HistoryPageComponent
    },
    {
        path: 'settings',
        component: SettingsPageComponent
    },
    {
        path: 'authorize/trakt',
        component: AuthorizeTraktComponent
    },
    {
        path: 'media/:id',
        component: MediaDetailsPageComponent
    },
    { path: '**', pathMatch: 'full', redirectTo: 'home' }
];

@NgModule({
    imports: [
        RouterModule.forRoot(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AppRoutingModule {
}
