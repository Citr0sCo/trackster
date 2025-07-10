import { IStat } from './stat.type';

export interface IStatModel {
    name: string;
    cpuUsage: IStat;
    memoryUsage: IStat;
    diskUsage: IStat;
}