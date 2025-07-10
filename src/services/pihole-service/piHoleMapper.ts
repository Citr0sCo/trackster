import { IPiHoleActivity } from './types/pihole-activity.type';

export class PiHoleMapper {

    public static mapActivity(payload: any): IPiHoleActivity {
        return {
            identifier: payload.Identifier,
            queriesToday: payload.Queries?.Total,
            blockedToday: payload.Queries?.Blocked,
            blockedPercentage: payload.Queries?.PercentBlocked,
            clients: payload.Clients?.Total
        };
    }

    public static mapActivities(payload: any): Array<IPiHoleActivity> {
        return payload.Response.Data.Activities.map(this.mapActivity);
    }
}