export interface IWebhook {
    identifier: string;
    apiKey: string;
    url: string;
    userIdentifier: string;
    provider: WebhookProvider;
}

export enum WebhookProvider {
    Unknown,
    Plex
}