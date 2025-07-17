export interface IMedia {
    identifier: string | null;
    mediaType: MediaType;
    title: string;
    parentTitle: string;
    grandParentTitle: string;
    year: number;
    tmdb: string;
    posterUrl: string;
    overview: string;
    watchedAt: Date;
}

export enum MediaType {
    Unknown,
    Movie,
    Episode
}