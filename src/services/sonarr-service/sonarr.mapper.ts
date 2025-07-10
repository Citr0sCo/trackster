import { ISonarrActivity } from './types/sonarr-activity.type';

export class SonarrMapper {

    public static mapActivity(payload: any): ISonarrActivity {
        return {
            totalNumberOfSeries: payload.TotalNumberOfSeries,
            totalNumberOfQueuedEpisodes: payload.TotalNumberOfQueuedEpisodes,
            totalNumberOfMissingEpisodes: payload.TotalNumberOfMissingEpisodes,
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

    public static mapWebsocketActivities(payload: any): ISonarrActivity {
        return this.mapActivity(payload.Response.Data);
    }
}