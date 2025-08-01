export interface IMedia {
    identifier: string | null;
    mediaType: string;
    title: string | null;
    slug: string | null;
    parentTitle: string | null;
    grandParentTitle: string | null;
    year: number;
    tmdb: string;
    posterUrl: string;
    overview: string;
    watchedAt: Date;
    seasonNumber: number;
    episodeNumber: number;
}

export enum MediaType {
    Unknown = 'Unknown',
    Movie = 'Movie',
    Episode = 'Episode'
}