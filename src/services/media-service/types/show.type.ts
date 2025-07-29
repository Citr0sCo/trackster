export interface IShow {
    identifier: string | null;
    title: string;
    parentTitle: string;
    grandParentTitle: string;
    year: number;
    tmdb: string;
    posterUrl: string;
    overview: string;
    watchedAt: Date;
    seasonNumber: number;
    episodeNumber: number;
}
