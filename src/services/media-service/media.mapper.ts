import {IMedia, MediaType} from "./types/media.type";
import {IWatchedMovie} from "../movie-service/types/watched-movie.type";
import {IWatchedEpisode} from "../show-service/types/watched-episode.type";

export class MediaMapper {
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

    public static fromEpisode(record: IWatchedEpisode) : IMedia {
        return {
            identifier: record.episode.season.show.identifier,
            title: record.episode.title,
            slug: record.episode.season.show.slug,
            parentTitle: record.episode.season.title,
            grandParentTitle: record.episode.season.show.title,
            watchedAt: record.watchedAt,
            mediaType: MediaType.Episode.toString(),
            overview: record.episode.season.show.overview,
            posterUrl: record.episode.season.show.posterUrl,
            tmdb: record.episode.season.show.tmdb,
            year: record.episode.season.show.year,
            seasonNumber: record.episode.season.number,
            episodeNumber: record.episode.number
        };
    }
}