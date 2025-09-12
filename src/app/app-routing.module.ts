import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from '../pages/home-page/home-page.component';
import {AuthorizeTraktComponent} from "../pages/authorize-trakt-page/authorize-trakt.component";
import { HistoryPageComponent } from '../pages/history-page/history-page.component';
import {SettingsPageComponent} from "../pages/settings-page/settings-page.component";
import {MovieDetailsPageComponent} from "../pages/movie-details-page/movie-details-page.component";
import {ShowDetailsPageComponent} from "../pages/show-details-page/show-details-page.component";
import { StatisticsPageComponent } from '../pages/statistics-page/statistics-page.component';
import {LoginPageComponent} from "../pages/login-page/login-page.component";
import {DashboardPageComponent} from "../pages/dashboard-page/dashboard-page.component";
import {RegisterPageComponent} from "../pages/register-page/register-page.component";
import {LogoutPageComponent} from "../pages/logout-page/logout-page.component";

const routes: Routes = [
    {
        path: 'login',
        component: LoginPageComponent
    },
    {
        path: 'register',
        component: RegisterPageComponent
    },
    {
        path: 'logout',
        component: LogoutPageComponent
    },
    {
        path: 'app',
        component: DashboardPageComponent,
        children: [
            {
                path: 'home',
                component: HomePageComponent
            },
            {
                path: 'history',
                component: HistoryPageComponent
            },
            {
                path: 'statistics',
                component: StatisticsPageComponent
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
                path: 'movies/:slug',
                component: MovieDetailsPageComponent
            },
            {
                path: 'shows/:slug/seasons/:seasonId/episodes/:episodeId',
                component: ShowDetailsPageComponent
            }
        ]
    },
    { path: '**', pathMatch: 'full', redirectTo: 'login' }
];

@NgModule({
    imports: [
        RouterModule.forRoot(routes, { useHash: true })
    ],
    exports: [
        RouterModule
    ]
})
export class AppRoutingModule {
}
