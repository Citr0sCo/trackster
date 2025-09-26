import {IMovie} from "./types/movie.type";

export class MovieMapper {
    public static map(movie: any) : IMovie {
        return {
            identifier: movie.Identifier,
            title: movie.Title,
            slug: movie.Slug,
            year: movie.Year,
            tmdb: movie.TMDB,
            posterUrl: movie.Poster,
            overview: movie.Overview,
            genres: movie.Genres.map((x: any) => {
                return {
                    identifier: x.Identifier,
                    name: x.Name,
                };
            }),
        };
    }
}