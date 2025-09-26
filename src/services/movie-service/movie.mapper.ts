import {IMedia, MediaType} from "./types/media.type";
import {IWatchedMovie} from "./types/watched-movie.type";
import {IWatchedShow} from "./types/watched-show.type";

export class MovieMapper {
    public static fromMovie(watchedMovie: IWatchedMovie) : IMedia {
        return {
            identifier: watchedMovie.movie.identifier,
            title: watchedMovie.movie.title,
            slug: watchedMovie.movie.slug,
            parentTitle: null,
            grandParentTitle: null,
            watchedAt: watchedMovie.watchedAt,
            mediaType: MediaType.Movie.toString(),
            overview: watchedMovie.movie.overview,
            posterUrl: watchedMovie.movie.posterUrl,
            tmdb: watchedMovie.movie.tmdb,
            year: watchedMovie.movie.year,
            seasonNumber: 0,
            episodeNumber: 0,
        };
    }

    public static fromShow(show: IWatchedShow) : IMedia {
        return {
            identifier: show.show.identifier,
            title: show.episode.title,
            slug: show.show.slug,
            parentTitle: show.season.title,
            grandParentTitle: show.show.title,
            watchedAt: show.watchedAt,
            mediaType: MediaType.Episode.toString(),
            overview: show.show.overview,
            posterUrl: show.show.posterUrl,
            tmdb: show.show.tmdb,
            year: show.show.year,
            seasonNumber: show.season.number,
            episodeNumber: show.episode.number
        };
    }
}