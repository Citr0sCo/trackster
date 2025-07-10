import { IRadarrActivity } from './types/radarr-activity.type';

export class RadarrMapper {

    public static mapActivity(payload: any): IRadarrActivity {
        return {
            totalNumberOfMovies: payload.TotalNumberOfMovies,
            totalNumberOfQueuedMovies: payload.TotalNumberOfQueuedMovies,
            totalMissingMovies: payload.TotalMissingMovies,
            health: payload.Health.map((x: any) => {
                return {
                    type: x.Type,
                    message: x.Message,
                    wikiUrl: x.WwkiUrl,
                    source: x.Source
                };
            })
        };
    }

    public static mapWebsocketActivities(payload: any): IRadarrActivity {
        return this.mapActivity(payload.Response.Data);
    }
}