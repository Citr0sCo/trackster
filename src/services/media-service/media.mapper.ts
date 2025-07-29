import {IMovie} from "./types/movie.type";
import {IMedia, MediaType} from "./types/media.type";
import {IShow} from "./types/show.type";

export class MediaMapper {
    public static fromMovie(movie: IMovie) : IMedia {
        return {
            identifier: movie.identifier,
            title: movie.title,
            parentTitle: null,
            grandParentTitle: null,
            watchedAt: movie.watchedAt,
            mediaType: MediaType.Movie,
            overview: movie.overview,
            posterUrl: movie.posterUrl,
            tmdb: movie.tmdb,
            year: movie.year,
        };
    }

    public static fromShow(show: IShow) : IMedia {
        return {
            identifier: show.identifier,
            title: show.title,
            parentTitle: show.parentTitle,
            grandParentTitle: show.grandParentTitle,
            watchedAt: show.watchedAt,
            mediaType: MediaType.Movie,
            overview: show.overview,
            posterUrl: show.posterUrl,
            tmdb: show.tmdb,
            year: show.year,
        };
    }
}