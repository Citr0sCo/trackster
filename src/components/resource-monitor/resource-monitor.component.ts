import { Component, Input, OnChanges } from '@angular/core';
import { IStatModel } from '../../services/stats-service/types/stat-model.type';

@Component({
    selector: 'resource-monitor',
    templateUrl: './resource-monitor.component.html',
    styleUrls: ['./resource-monitor.component.scss'],
    standalone: false
})
export class ResourceMonitorComponent implements OnChanges {

    @Input()
    public allStats: Array<IStatModel> = new Array<IStatModel>();

    public stats: IStatModel | null = null;

    public ngOnChanges(): void {

        const homeAppStats = this.allStats.find((x) => x.name.indexOf('home-app') > -1);

        this.stats = {
            cpuUsage: {
                percentage: this.allStats.map((y) => y.cpuUsage).reduce((y, { percentage }) => y + percentage, 0),
                total: homeAppStats?.cpuUsage.total ?? 0,
                used: homeAppStats?.cpuUsage.used ?? 0
            },
            memoryUsage: {
                percentage: this.allStats.map((y) => y.memoryUsage).reduce((y, { percentage }) => y + percentage, 0),
                total: homeAppStats?.memoryUsage.total ?? 0,
                used: this.allStats.map((y) => y.memoryUsage).reduce((y, { used }) => y + used, 0)
            },
            diskUsage: homeAppStats?.diskUsage ?? {
                percentage: 0,
                used: 0,
                total: 0
            },
            name: homeAppStats?.name ?? 'app'
        };
    }

    public bytesToGigaBytes(valueInBytes: number): number {
        return Math.round((valueInBytes / 1000000000) * 100) / 100;
    }

    public roundToTwoDecmalPoints(valueInBytes: number): number {
        return Math.round((valueInBytes) * 100) / 100;
    }
}
