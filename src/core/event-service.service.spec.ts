import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { NgZone } from '@angular/core';
import { StreamService } from './event-service.service';
import { AuthenticationService } from '../services/authentication-service/authentication.service';

class FakeEventSource {
    static instances: FakeEventSource[] = [];

    public onmessage: ((event: MessageEvent) => void) | null = null;
    public onerror: ((event: Event) => void) | null = null;
    public closed = false;

    constructor(public url: string) {
        FakeEventSource.instances.push(this);
    }

    public close(): void {
        this.closed = true;
    }
}

describe('StreamService', () => {
    beforeEach(() => {
        FakeEventSource.instances = [];
        vi.stubGlobal('EventSource', FakeEventSource);
    });

    afterEach(() => {
        vi.unstubAllGlobals();
    });

    const createService = () => {
        const authService = { getAuthToken: () => 'session-id' } as unknown as AuthenticationService;
        const run = vi.fn((fn: () => void) => fn());
        const zone = { run } as unknown as NgZone;

        return { service: new StreamService(authService, zone), run };
    };

    it('connects with the session token and forwards messages through the Angular zone', () => {
        const { service, run } = createService();
        const received: string[] = [];

        service.startStream('http://api/media/import', true).subscribe(data => received.push(data));

        const source = FakeEventSource.instances[0];

        expect(source.url).toBe('http://api/media/import?token=session-id&isDebug=true');

        source.onmessage!(new MessageEvent('message', { data: '{"Data":"[INFO] - Starting"}' }));

        expect(received).toEqual(['{"Data":"[INFO] - Starting"}']);
        expect(run).toHaveBeenCalledTimes(1);
    });

    it('reports stream failures and stops reconnecting', () => {
        const { service } = createService();
        const errors: unknown[] = [];

        service.startStream('http://api/media/import', false).subscribe({ error: error => errors.push(error) });

        const source = FakeEventSource.instances[0];
        source.onerror!(new Event('error'));

        expect(errors).toHaveLength(1);
        expect(source.closed).toBe(true);
    });

    it('closes only the connection it owns when a stream is torn down', () => {
        const { service } = createService();
        const first = service.startStream('http://api/media/import', false).subscribe();
        const second = service.startStream('http://api/media/import', false).subscribe();

        first.unsubscribe();

        expect(FakeEventSource.instances[0].closed).toBe(true);
        expect(FakeEventSource.instances[1].closed).toBe(false);

        second.unsubscribe();

        expect(FakeEventSource.instances[1].closed).toBe(true);
    });
});
