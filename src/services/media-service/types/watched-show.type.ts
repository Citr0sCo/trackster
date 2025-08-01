import {IMovie} from "./movie.type";
import {IShow} from "./show.type";
import {ISeason} from "./season.type";
import {IEpisode} from "./episode.type";

export interface IWatchedShow {
    show: IShow;
    season: ISeason;
    episode: IEpisode;
    watchedAt: Date;
}
