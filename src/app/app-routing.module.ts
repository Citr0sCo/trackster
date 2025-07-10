import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from '../pages/home-page/home-page.component';
import { FuelPricesPageComponent } from '../pages/fuel-prices-page/fuel-prices-page.component';
import { UpdateDockerAppsPageComponent } from '../pages/update-docker-apps-page/update-docker-apps-page.component';

const routes: Routes = [
    {
        path: 'home',
        component: HomePageComponent
    },
    {
        path: 'fuel-prices',
        component: FuelPricesPageComponent
    },
    {
        path: 'update-docker-apps',
        component: UpdateDockerAppsPageComponent
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
