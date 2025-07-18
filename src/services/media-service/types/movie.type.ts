export interface IMovie {
    identifier: string | null;
    title: string;
    year: number;
    tmdb: string;
    posterUrl: string;
    overview: string;
    watchedAt: Date;
}
