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

    public static fromApi(item: any) : IMedia {
        return {
            identifier: item.Identifier,
            mediaType: item.Type,
            title: item.Title,
            slug: item.Slug,
            parentTitle: item.ParentTitle,
            grandParentTitle: item.GrandParentTitle,
            year: item.Year,
            tmdb: item.TMDB,
            posterUrl: item.Poster,
            overview: item.Overview,
            watchedAt: new Date(item.WatchedAt),
            seasonNumber: item.SeasonNumber,
            episodeNumber: item.EpisodeNumber,
        };
    }

    public static fromApiArray(media: Array<any>) : Array<IMedia> {
        return media.map((item: any) => MediaMapper.fromApi(item));
    }
}
