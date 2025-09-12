import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { mapNetworkError } from '../../core/map-network-error';
import { Provider } from "../../core/providers.enum";
import { ImportType } from "../../core/import-type.enum";
import { IMovie } from "./types/movie.type";
import { IShow } from "./types/show.type";
import { IMedia } from "./types/media.type";
import { IWatchedMovie } from "./types/watched-movie.type";
import { IWatchedShow } from "./types/watched-show.type";
import { IEpisode } from "./types/episode.type";
import { ISeason } from "./types/season.type";
import { IWatchedEpisode } from "./types/watched-episode.type";
import { IStats } from './types/stats.type';
import { AuthenticationService } from '../authentication-service/authentication.service';

@Injectable()
export class MediaRepository {

    private _httpClient: HttpClient;
    private _authService: AuthenticationService;

    constructor(httpClient: HttpClient, authService: AuthenticationService) {
        this._httpClient = httpClient;
        this._authService = authService;
    }

    public getPosters(): Observable<Array<string>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/posters`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    return response;
                })
            );
    }

    public getAllMoviesFor(username: string): Observable<Array<IWatchedMovie>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/movies?username=${username}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    return response.WatchedMovies.map((movie: any) => {
                        return {
                            movie: {
                                identifier: movie.Movie.Identifier,
                                title: movie.Movie.Title,
                                slug: movie.Movie.Slug,
                                year: movie.Movie.Year,
                                tmdb: movie.Movie.TMDB,
                                posterUrl: movie.Movie.Poster,
                                overview: movie.Movie.Overview,
                            },
                            watchedAt: movie.WatchedAt
                        };
                    });
                })
            );
    }

    public getAllShowsFor(username: string): Observable<Array<IWatchedShow>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows?username=${username}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    return response.WatchedShows.map((show: any) => {
                        return {
                            show: {
                                identifier: show.Show.Identifier,
                                title: show.Show.Title,
                                slug: show.Show.Slug,
                                year: show.Show.Year,
                                tmdb: show.Show.TMDB,
                                posterUrl: show.Show.Poster,
                                overview: show.Show.Overview,
                            },
                            season: {
                                identifier: show.Season.Identifier,
                                title: show.Season.Title,
                                number: show.Season.Number
                            },
                            episode: {
                                identifier: show.Episode.Identifier,
                                title: show.Episode.Title,
                                number: show.Episode.Number
                            },
                            watchedAt: show.WatchedAt
                        };
                    });
                })
            );
    }

    public getHistoryForUser(username: string, results: number, page: number): Observable<Array<IMedia>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/history?username=${username}&results=${results}&page=${page}`).pipe()
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    return response.Media.map((media: any) => {
                        return {
                            identifier: media.Identifier,
                            mediaType: media.Type,
                            title: media.Title,
                            slug: media.Slug,
                            parentTitle: media.ParentTitle,
                            grandParentTitle: media.GrandParentTitle,
                            year: media.Year,
                            tmdb: media.TMDB,
                            posterUrl: media.Poster,
                            overview: media.Overview,
                            watchedAt: new Date(media.WatchedAt),
                            seasonNumber: media.SeasonNumber,
                            episodeNumber: media.EpisodeNumber,
                        };
                    });
                })
            );
    }

    public getStats(username: string): Observable<IStats> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/stats?username=${username}`).pipe()
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    return {
                        totalWatched: response.Total,
                        totalMoviesWatched: response.MoviesWatched,
                        totalEpisodesWatched: response.EpisodesWatched,
                    };
                })
            );
    }

    public getStatsForCalendar(username: string, daysInThePast: number): Observable<Array<any>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/media/stats/calendar?username=${username}&daysInThePast=${daysInThePast}`).pipe()
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    return response.Stats;
                })
            );
    }

    public importFromTrakt(username: string, debug: boolean): Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/media/import`, { Type: ImportType.Trakt, Username: username, Debug: debug })
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
            );
    }

    public getMovieBySlug(slug: string): Observable<IMovie> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/movies/${slug}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    const media = response.Movie;
                    return {
                        identifier: media.Identifier,
                        mediaType: media.MediaType,
                        title: media.Title,
                        slug: media.Slug,
                        parentTitle: media.ParentTitle,
                        grandParentTitle: media.GrandParentTitle,
                        year: media.Year,
                        tmdb: media.TMDB,
                        posterUrl: media.Poster,
                        overview: media.Overview,
                        seasonNumber: media.SeasonNumber,
                        episodeNumber: media.EpisodeNumber,
                    };
                })
            );
    }

    public getShowBySlug(slug: string): Observable<IShow> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${slug}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    const show = response.Show;
                    return {
                        identifier: show.Identifier,
                        title: show.Title,
                        slug: show.Slug,
                        year: show.Year,
                        tmdb: show.TMDB,
                        posterUrl: show.Poster,
                        overview: show.Overview,
                    };
                })
            );
    }

    public getSeasonById(identifier: string, seasonNumber: number): Observable<ISeason> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    const season = response.Season;
                    return {
                        identifier: season.Identifier,
                        title: season.Title,
                        number: season.Number
                    };
                })
            );
    }

    public getEpisodeById(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}/episodes/${episodeNumber}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    const episode = response.Episode;
                    return {
                        identifier: episode.Identifier,
                        title: episode.Title,
                        number: episode.Number
                    };
                })
            );
    }

    public updateEpisodeById(identifier: string, seasonNumber: number, episodeNumber: number): Observable<IEpisode> {
        return this._httpClient.patch(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}/episodes/${episodeNumber}`, {})
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    const episode = response.Episode;
                    return {
                        identifier: episode.Identifier,
                        title: episode.Title,
                        number: episode.Number
                    };
                })
            );
    }

    public getEpisodeWatchHistory(username: string, identifier: string, seasonNumber: number, episodeNumber: number): Observable<Array<IWatchedEpisode>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/shows/${identifier}/seasons/${seasonNumber}/episodes/${episodeNumber}/history?username=${username}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    const episodes = response.WatchedEpisodes;
                    return episodes.map((episode: any) => {
                        return {
                            watchedAt: episode.WatchedAt
                        };
                    });
                })
            );
    }

    public getMovieWatchHistoryById(username: string, identifier: string): Observable<Array<IWatchedMovie>> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/movies/${identifier}/history?username=${username}`)
            .pipe(
                mapNetworkError(),
                tap((tap) => {
                }, (error) => {
                    if (error.exception.status === 401) {
                        this._authService.logout()
                            .subscribe(() => {
                                window.location.href = "/#/login";
                            });
                    }
                }),
                map((response: any) => {
                    const episodes = response.WatchHistory;
                    return episodes.map((episode: any) => {
                        return {
                            watchedAt: episode.WatchedAt
                        };
                    });
                })
            );
    }
}