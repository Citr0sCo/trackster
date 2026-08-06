import { enableProdMode, provideZoneChangeDetection } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { init, trackViews } from 'swetrix';

if (environment.production) {
    enableProdMode();
    const analytics = environment.analytics as { projectId: string; apiUrl: string } | undefined;
    if (analytics?.projectId && analytics.apiUrl) {
        init(analytics.projectId, {
            apiURL: analytics.apiUrl,
            respectDNT: true
        });
        void trackViews().catch(() => undefined);
    }
}

platformBrowserDynamic().bootstrapModule(AppModule, { applicationProviders: [provideZoneChangeDetection()], })
    .catch((err) => console.error(err));
