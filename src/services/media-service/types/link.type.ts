export interface ILink {
    identifier: string | null;
    containerName: string | null;
    name: string;
    url: string;
    host: string;
    port: number;
    isSecure: boolean;
    iconUrl: string;
    category: string;
    sortOrder: string;
}
