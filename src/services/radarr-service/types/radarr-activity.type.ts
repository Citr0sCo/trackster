export interface IRadarrActivity {
    totalNumberOfMovies: number;
    totalNumberOfQueuedMovies: number;
    totalMissingMovies: number;
    health: Array<IRadarrHealth>;
}

export interface IRadarrHealth {
    type: string;
    message: string;
    wikiUrl: string;
    source: string;
}