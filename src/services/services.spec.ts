import { EventService } from './event-service/event.service';
import { LinkMapper } from './media-service/link.mapper';
import { MediaMapper } from './media-service/media.mapper';
import { MediaType } from './media-service/types/media.type';
import { MovieMapper } from './movie-service/movie.mapper';
import { ShowMapper } from './show-service/show.mapper';
import { UserMapper } from './user-service/user.mapper';

describe('service utilities', () => {
    it('maps links from API naming to frontend naming', () => {
        const link = LinkMapper.mapSingle({
            Identifier: 'link-1',
            containerName: 'Plex',
            Name: 'Watch',
            Url: 'https://example.test/watch',
            Host: 'example.test',
            Port: 443,
            IsSecure: true,
            IconUrl: '/icon.svg',
            Category: 'stream',
            SortOrder: 2
        });

        expect(LinkMapper.map([{
            Identifier: 'link-1',
            containerName: 'Plex',
            Name: 'Watch',
            Url: 'https://example.test/watch',
            Host: 'example.test',
            Port: 443,
            IsSecure: true,
            IconUrl: '/icon.svg',
            Category: 'stream',
            SortOrder: 2
        }])).toEqual([link]);
        expect(link).toEqual({
            identifier: 'link-1',
            containerName: 'Plex',
            name: 'Watch',
            url: 'https://example.test/watch',
            host: 'example.test',
            port: 443,
            isSecure: true,
            iconUrl: '/icon.svg',
            category: 'stream',
            sortOrder: 2
        });
    });

    it('maps movies and episodes into the common media model', () => {
        const watchedAt = new Date('2026-01-02T03:04:05Z');
        const movie = {
            identifier: 'movie-1', title: 'Movie', slug: 'movie', overview: 'Overview',
            posterUrl: '/movie.jpg', tmdb: 'tmdb-movie', year: 2026
        };
        const show = {
            identifier: 'show-1', title: 'Show', slug: 'show', overview: 'Show overview',
            posterUrl: '/show.jpg', tmdb: 'tmdb-show', year: 2025
        };
        const episode = {
            title: 'Episode', number: 3,
            season: { title: 'Season 2', number: 2, show }
        };

        expect(MediaMapper.fromMovie({ movie, watchedAt } as any)).toEqual({
            identifier: 'movie-1', title: 'Movie', slug: 'movie',
            parentTitle: null, grandParentTitle: null, watchedAt,
            mediaType: MediaType.Movie.toString(), overview: 'Overview',
            posterUrl: '/movie.jpg', tmdb: 'tmdb-movie', year: 2026,
            seasonNumber: 0, episodeNumber: 0
        });
        expect(MediaMapper.fromEpisode({ episode, watchedAt } as any)).toEqual({
            identifier: 'show-1', title: 'Episode', slug: 'show',
            parentTitle: 'Season 2', grandParentTitle: 'Show', watchedAt,
            mediaType: MediaType.Episode.toString(), overview: 'Show overview',
            posterUrl: '/show.jpg', tmdb: 'tmdb-show', year: 2025,
            seasonNumber: 2, episodeNumber: 3
        });
    });

    it('maps movie and show genres', () => {
        const apiRecord = {
            Identifier: 'media-1', Title: 'Title', Slug: 'title', Year: 2026,
            TMDB: 'tmdb-1', Poster: '/poster.jpg', Overview: 'Overview',
            Genres: [{ Identifier: 'genre-1', Name: 'Drama' }]
        };

        const expected = {
            identifier: 'media-1', title: 'Title', slug: 'title', year: 2026,
            tmdb: 'tmdb-1', posterUrl: '/poster.jpg', overview: 'Overview',
            genres: [{ identifier: 'genre-1', name: 'Drama' }]
        };

        expect(MovieMapper.map(apiRecord)).toEqual(expected);
        expect(ShowMapper.map(apiRecord)).toEqual(expected);
    });

    it('maps nested show seasons and episodes', () => {
        const show = {
            Identifier: 'show-1', Title: 'Show', Slug: 'show', Year: 2026,
            TMDB: 'tmdb-show', Poster: '/show.jpg', Overview: 'Overview', Genres: []
        };
        const season = { Identifier: 'season-1', Title: 'Season 1', Number: 1, Show: show };
        const episode = { Identifier: 'episode-1', Title: 'Pilot', Number: 1, Season: season };

        expect(ShowMapper.mapSeason(season)).toEqual({
            identifier: 'season-1', title: 'Season 1', number: 1,
            show: ShowMapper.map(show)
        });
        expect(ShowMapper.mapEpisode(episode)).toEqual({
            identifier: 'episode-1', title: 'Pilot', number: 1,
            season: ShowMapper.mapSeason(season)
        });
    });

    it('maps users in both directions', () => {
        const createdAt = new Date('2026-01-01T00:00:00Z');
        const apiUser = {
            Identifier: 'user-1', Username: 'alice', Email: 'alice@example.test', CreatedAt: createdAt,
            ThirdPartyIntegrations: [{ Identifier: 'integration-1', Provider: 2 }]
        };
        const user = UserMapper.map(apiUser);

        expect(user).toEqual({
            identifier: 'user-1', username: 'alice', email: 'alice@example.test', createdAt,
            thirdPartyIntegrations: [{ identifier: 'integration-1', provider: 2 }]
        });
        expect(UserMapper.mapRecord(user)).toEqual(apiUser);
    });

    it('publishes true and false page-scroll states', () => {
        const service = new EventService();
        const states: boolean[] = [];
        service.scrolledToBottom.subscribe((state) => states.push(state));

        service.scrolledToBottomOfThePage();
        service.notScrolledToBottomOfThePage();

        expect(states).toEqual([true, false]);
    });
});
