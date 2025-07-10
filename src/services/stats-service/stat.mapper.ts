import { IStatResponse } from './types/stat.response';

export class StatMapper {

    public static   map(response: any): IStatResponse {
        return {
            stats: response.Stats.map((stat: any) => {
                return {
                    name: stat.Name,
                    cpuUsage: {
                        percentage: stat.CpuUsage?.Percentage,
                        total: stat.CpuUsage?.Total,
                        used: stat.CpuUsage?.Used
                    },
                    memoryUsage: {
                        percentage: stat.MemoryUsage?.Percentage,
                        total: stat.MemoryUsage?.Total,
                        used: stat.MemoryUsage?.Used
                    },
                    diskUsage: {
                        percentage: stat.DiskUsage?.Percentage,
                        total: stat.DiskUsage?.Total,
                        used: stat.DiskUsage?.Used
                    }
                };
            })
        };
    }
}