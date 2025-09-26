import {IShow} from "./show.type";

export interface ISeason {
    identifier: string;
    title: string;
    number: number;
    show: IShow;
}
