import { IMedia, MediaType } from './types/media.type';

export class MediaLink {
    public static for(media: IMedia): Array<string> {
        if (media.mediaType === MediaType.Movie) {
            return ['/app/movies', media.slug ?? ''];
        }

        return [
            '/app/shows',
            media.slug ?? '',
            'seasons',
            `${media.seasonNumber}`,
            'episodes',
            `${media.episodeNumber}`
        ];
    }
}
