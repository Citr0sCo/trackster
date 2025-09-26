import {IEpisode} from "./types/episode.type";
import {IShow} from "./types/show.type";
import {ISeason} from "./types/season.type";

export class ShowMapper {

    static map(show: any) : IShow {
        return {
            identifier: show.Identifier,
            title: show.Title,
            slug: show.Slug,
            year: show.Year,
            tmdb: show.TMDB,
            posterUrl: show.Poster,
            overview: show.Overview,
            genres: show.Genres.map((x: any) => {
                return {
                    identifier: x.Identifier,
                    name: x.Name,
                };
            }),
        };
    }

    static mapSeason(season: any) : ISeason {
        return {
            identifier: season.Identifier,
            title: season.Title,
            number: season.Number,
            show: this.map(season.Show),
        };
    }

    public static mapEpisode(episode: any) : IEpisode {
        return {
            identifier: episode.Identifier,
            title: episode.Title,
            number: episode.Number,
            season: this.mapSeason(episode.Season),
        };
    }
}