import {IGenre} from "../../media-service/types/genre.type";

export interface IShow {
    identifier: string;
    title: string;
    slug: string;
    year: number;
    tmdb: string;
    posterUrl: string;
    overview: string;
    genres: Array<IGenre>;
}
