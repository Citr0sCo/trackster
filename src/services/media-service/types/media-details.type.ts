export interface ICastMember {
    name: string;
    character: string;
    profileUrl: string;
}

export interface IMediaDetails {
    tagline: string;
    backdropUrl: string;
    imdbUrl: string;
    traktUrl: string;
    releaseDate: string | null;
    runtime: number;
    rating: number;
    ratingCount: number;
    status: string;
    seasonCount: number;
    episodeCount: number;
    cast: Array<ICastMember>;
}
