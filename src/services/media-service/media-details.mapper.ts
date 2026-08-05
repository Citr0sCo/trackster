import {IMediaDetails} from './types/media-details.type';

export class MediaDetailsMapper {
    public static map(details: any): IMediaDetails {
        return {
            tagline: details?.Tagline || '',
            backdropUrl: details?.Backdrop || '',
            imdbUrl: details?.ImdbUrl || '',
            traktUrl: details?.TraktUrl || '',
            releaseDate: details?.ReleaseDate || null,
            runtime: details?.Runtime || 0,
            rating: details?.Rating || 0,
            ratingCount: details?.RatingCount || 0,
            status: details?.Status || '',
            seasonCount: details?.SeasonCount || 0,
            episodeCount: details?.EpisodeCount || 0,
            cast: (details?.Cast || []).map((cast: any) => ({
                name: cast.Name,
                character: cast.Character,
                profileUrl: cast.Profile || '',
            })),
        };
    }
}
