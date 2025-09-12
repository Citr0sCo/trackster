import { Injectable } from '@angular/core';
import { WebhookRepository } from './webhook.repository';
import { Observable, of, tap } from 'rxjs';
import { IWebhook } from './types/webhook.type';

@Injectable()
export class WebhookService {

    private _repository: WebhookRepository;

    constructor(repository: WebhookRepository) {
        this._repository = repository;
    }

    public getWebhook(userReference: string): Observable<IWebhook> {
        return this._repository.getWebhook(userReference);
    }

    public createWebhook(webhook: IWebhook): Observable<IWebhook> {

        const request = {
            UserIdentifier: webhook.userIdentifier,
            Provider: webhook.provider,
        };

        return this._repository.createWebhook(request);
    }
}
