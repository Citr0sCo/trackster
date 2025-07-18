import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from '../pages/home-page/home-page.component';
import {AuthorizeTraktComponent} from "../pages/authorize-trakt-page/authorize-trakt.component";
import { HistoryPageComponent } from '../pages/history-page/history-page.component';

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
        path: 'authorize/trakt',
        component: AuthorizeTraktComponent
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
