import {ISeason} from "./season.type";

export interface IEpisode {
    identifier: string;
    title: string;
    number: number;
    season: ISeason;
}
