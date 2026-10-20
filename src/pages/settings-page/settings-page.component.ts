import { Component, OnDestroy, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { StreamService } from "../../core/event-service.service";
import { environment } from "../../environments/environment";
import { round } from "@popperjs/core/lib/utils/math";
import { UserService } from '../../services/user-service/user.service';
import { IUser } from '../../services/user-service/types/user.type';
import { WebhookService } from "../../services/webhook-service/webhook.service";
import { IWebhook, WebhookProvider } from "../../services/webhook-service/types/webhook.type";
import { Provider } from '../../core/providers.enum';

const MaxLogLineLength = 300;

interface IImportProgress {
    Data: string;
    Total: number;
    Processed: number;
    Type: string;
}

@Component({
    selector: 'settings-page',
    templateUrl: './settings-page.component.html',
    styleUrls: ['./settings-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SettingsPageComponent implements OnInit, OnDestroy {

    public isImporting: boolean = false;
    public isCreatingWebhook: boolean = false;
    public isDebug: boolean = false;
    public progress: Array<string> = [];
    public mediaType: string = '';
    public processed: number = 0;
    public total: number = 0;
    public readonly round = round;
    public user: IUser | null = null;
    public webhook: IWebhook | null = null;
    public readonly Provider = Provider;

    private syncSubscription: Subscription | null = null;
    private readonly _destroy: Subject<void> = new Subject();
    private readonly _streamService: StreamService;
    private readonly _userService: UserService;
    private readonly _webhookService: WebhookService;

    constructor(streamService: StreamService, userService: UserService, webhookService: WebhookService) {
        this._streamService = streamService;
        this._userService = userService;
        this._webhookService = webhookService;
    }

    public ngOnInit(): void {

        this._userService.getUserBySession()
            .pipe(takeUntil(this._destroy))
            .subscribe((user) => {
                this.user = user;

                this._webhookService.getWebhook(this.user.identifier)
                    .pipe(takeUntil(this._destroy))
                    .subscribe((webhook) => {
                        this.webhook = webhook;
                    });
            });
    }

    public syncWithTrakt(): void {
        if (this.isImporting) {
            return;
        }

        this.isImporting = true;
        this.progress = [];
        this.processed = 0;
        this.total = 0;
        this.mediaType = '';

        this.syncSubscription = this._streamService.startStream(`${environment.apiBaseUrl}/api/media/import`, this.isDebug)
            .pipe(takeUntil(this._destroy))
            .subscribe({
                next: (data: string) => this.appendProgress(data),
                error: () => this.stopSync('Sync stopped unexpectedly. Check that you are signed in and connected to Trakt.')
            });
    }

    private appendProgress(data: string): void {
        let progress: IImportProgress;

        try {
            progress = JSON.parse(data) as IImportProgress;
        } catch {
            this.stopSync('The server sent an unexpected response while syncing with Trakt.');
            return;
        }

        if (progress.Data) {
            this.progress.push(`${this.toLogLine(progress.Data)}\r`);
        }

        if (progress.Total > 0) {
            this.total = progress.Total;
        }

        if (progress.Processed > 0) {
            this.processed = progress.Processed;
        }

        if (progress.Type) {
            this.mediaType = progress.Type;
        }

        if (progress.Data?.includes('DONE!')) {
            this.isImporting = false;
            this.syncSubscription?.unsubscribe();
        }

        this.scrollLogToBottom();
    }

    private stopSync(message: string): void {
        if (!this.isImporting) {
            return;
        }

        this.isImporting = false;
        this.syncSubscription?.unsubscribe();

        const lastLine = this.progress[this.progress.length - 1] ?? '';

        if (!lastLine.includes('[Error]')) {
            this.progress.push(`[Error] - ${message}\r`);
        }

        this.scrollLogToBottom();
    }

    // Server messages are free-form strings; keep one line each and cap them so a stray
    // response body can never flood the log window.
    private toLogLine(value: string): string {
        const singleLine = value.replace(/\s+/g, ' ').trim();

        return singleLine.length <= MaxLogLineLength ? singleLine : `${singleLine.slice(0, MaxLogLineLength)}…`;
    }

    private scrollLogToBottom(): void {
        setTimeout(() => {
            const logWindowElement = document.querySelector('.log-window');

            if (logWindowElement) {
                logWindowElement.scrollTo(0, logWindowElement.scrollHeight);
            }
        }, 100);
    }

    public saveSettings(): void {

    }

    public generateWebhookUrl(): void {
        this.isCreatingWebhook = true;

        const webhook = {
            userIdentifier: this.user?.identifier,
            provider: WebhookProvider.Plex
        } as IWebhook;

        this._webhookService.createWebhook(webhook)
            .pipe(takeUntil(this._destroy))
            .subscribe((webhook) => {
                this.webhook = webhook;
                this.isCreatingWebhook = false;
            });
    }

    public hasThirdPartyIntegration(provider: Provider): boolean {
        return !!this.user?.thirdPartyIntegrations.find(x => x.provider === provider);
    }

    public ngOnDestroy(): void {
        this._destroy.next();
        this._destroy.complete();
        this.syncSubscription?.unsubscribe();
    }
}
