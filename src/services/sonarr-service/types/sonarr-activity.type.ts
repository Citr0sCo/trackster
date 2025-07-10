export interface ISonarrActivity {
    totalNumberOfSeries: number;
    totalNumberOfQueuedEpisodes: number;
    totalNumberOfMissingEpisodes: number;
    health: Array<ISonarrHealth>;
}

export interface ISonarrHealth {
    type: string;
    message: string;
    wikiUrl: string;
    source: string;
}