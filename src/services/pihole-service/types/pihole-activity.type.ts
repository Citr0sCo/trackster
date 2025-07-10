export interface IPiHoleActivity {
    identifier: string;
    queriesToday: number;
    blockedToday: number;
    blockedPercentage: number;
    clients: number;
}