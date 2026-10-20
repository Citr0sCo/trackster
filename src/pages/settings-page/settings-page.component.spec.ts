import { describe, expect, it, vi } from 'vitest';
import { Subject } from 'rxjs';
import { SettingsPageComponent } from './settings-page.component';
import { StreamService } from '../../core/event-service.service';
import { UserService } from '../../services/user-service/user.service';
import { WebhookService } from '../../services/webhook-service/webhook.service';

describe('SettingsPageComponent Trakt sync', () => {
    const createComponent = () => {
        const events = new Subject<string>();
        const streamService = { startStream: vi.fn(() => events.asObservable()) } as unknown as StreamService;
        const userService = { getUserBySession: vi.fn() } as unknown as UserService;
        const webhookService = { getWebhook: vi.fn() } as unknown as WebhookService;

        const component = new SettingsPageComponent(streamService, userService, webhookService);

        return { component, events, streamService };
    };

    const message = (data: string, extra: Record<string, unknown> = {}) =>
        JSON.stringify({ Data: data, Total: 0, Processed: 0, Type: '', ...extra });

    it('reports progress and stops loading once the server says it is done', () => {
        const { component, events } = createComponent();

        component.syncWithTrakt();

        expect(component.isImporting).toBe(true);

        events.next(message('[INFO] - Starting Trakt import...'));
        events.next(message('[INFO] - Will process 2 movies.', { Total: 2, Processed: 1, Type: 'Movie' }));

        expect(component.progress).toEqual(['[INFO] - Starting Trakt import...\r', '[INFO] - Will process 2 movies.\r']);
        expect(component.total).toBe(2);
        expect(component.processed).toBe(1);
        expect(component.mediaType).toBe('Movie');
        expect(component.isImporting).toBe(true);

        events.next(message('[INFO] - DONE!'));

        expect(component.isImporting).toBe(false);
        expect(events.observed).toBe(false);
    });

    it('shows an error line instead of the raw body when the server sends something that is not JSON', () => {
        const { component, events } = createComponent();

        component.syncWithTrakt();
        events.next('<!DOCTYPE html><html><script>window.blocked = true;</script></html>');

        expect(component.isImporting).toBe(false);
        expect(component.progress).toEqual(['[Error] - The server sent an unexpected response while syncing with Trakt.\r']);
        expect(component.progress.join('')).not.toContain('<script');
        expect(events.observed).toBe(false);
    });

    it('never floods the log with a long single message', () => {
        const { component, events } = createComponent();

        component.syncWithTrakt();
        events.next(message(`[Error] - ${'function () { return 1; } '.repeat(200)}`));

        expect(component.progress).toHaveLength(1);
        expect(component.progress[0].length).toBeLessThanOrEqual(302);
        expect(component.progress[0]).toContain('…');
    });

    it('marks the sync as failed when the stream breaks before it finishes', () => {
        const { component, events } = createComponent();

        component.syncWithTrakt();
        events.next(message('[INFO] - Starting Trakt import...'));
        events.error(new Error('connection lost'));

        expect(component.isImporting).toBe(false);
        expect(component.progress.at(-1)).toContain('[Error] - Sync stopped unexpectedly');
    });

    it('does not add a second error line when the server already reported the failure', () => {
        const { component, events } = createComponent();

        component.syncWithTrakt();
        events.next(message('[Error] - Failed to read watched movies from Trakt: Trakt returned 403 (Forbidden).'));
        events.error(new Error('stream closed'));

        expect(component.progress).toEqual(['[Error] - Failed to read watched movies from Trakt: Trakt returned 403 (Forbidden).\r']);
    });

    it('does not report an error when the stream closes after a successful sync', () => {
        const { component, events } = createComponent();

        component.syncWithTrakt();
        events.next(message('[INFO] - DONE!'));
        events.error(new Error('stream closed after completion'));

        expect(component.progress).toEqual(['[INFO] - DONE!\r']);
    });

    it('starts a fresh run when triggered again and ignores overlapping runs', () => {
        const { component, events, streamService } = createComponent();

        component.syncWithTrakt();
        events.next(message('[INFO] - Starting Trakt import...', { Total: 5, Processed: 5 }));
        events.next(message('[INFO] - DONE!'));

        component.syncWithTrakt();

        expect(streamService.startStream).toHaveBeenCalledTimes(2);
        expect(component.progress).toEqual([]);
        expect(component.total).toBe(0);
        expect(component.processed).toBe(0);

        component.isImporting = true;
        component.syncWithTrakt();

        expect(streamService.startStream).toHaveBeenCalledTimes(2);
    });
});
