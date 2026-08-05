import { HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { ErrorCodes, mapNetworkError, UserError } from './map-network-error';
import { Session } from './session';
import { Stack } from './stack';
import { TerminalParser } from './terminal-parser';

describe('core utilities', () => {
    describe('Stack', () => {
        it('keeps last-in-first-out order and reports its top item', () => {
            const stack = new Stack<string>();

            expect(stack.size()).toBe(0);
            expect(stack.peek()).toBeUndefined();

            stack.push('first');
            stack.push('second');

            expect(stack.size()).toBe(2);
            expect(stack.peek()).toBe('second');
            expect(stack.pop()).toBe('second');
            expect(stack.pop()).toBe('first');
            expect(stack.pop()).toBeUndefined();
        });
    });

    describe('Session', () => {
        it('returns the identifier supplied at construction', () => {
            expect(new Session('session-123').identifier()).toBe('session-123');
        });
    });

    describe('TerminalParser', () => {
        it('returns empty output unchanged', () => {
            expect(new TerminalParser('').toHtml()).toBe('');
        });

        it('converts terminal colors, line breaks, and escaped quotes', () => {
            const output = '\u001b[0;36mcyan\u001b[0m\\n'
                + '\u001b[0;32mgreen\u001b[0m \\\"quoted\\\"';

            expect(new TerminalParser(output).toHtml()).toBe(
                '<span class="text-cyan">cyan</span><br />'
                + '<span class="text-green">green</span> "quoted"'
            );
        });
    });

    describe('UserError', () => {
        it('uses the provided error and exposes its message as a string', () => {
            const cause = new Error('cause');
            const error = new UserError('visible message', ErrorCodes.APP_EXCEPTION, cause);

            expect(error.name).toBe('UserError');
            expect(error.code).toBe(ErrorCodes.APP_EXCEPTION);
            expect(error.exception).toBe(cause);
            expect(error.toString()).toBe('visible message');
        });
    });

    describe('mapNetworkError', () => {
        const networkError = (status: number, error?: unknown) =>
            new HttpErrorResponse({ status, statusText: 'failure', error });

        const expectMappedError = (response: HttpErrorResponse, code: ErrorCodes, message: string) => {
            let actual: unknown;

            throwError(() => response).pipe(mapNetworkError()).subscribe({
                error: (error) => actual = error
            });

            expect(actual).toBeInstanceOf(UserError);
            expect((actual as UserError).code).toBe(code);
            expect((actual as UserError).message).toBe(message);
            expect((actual as UserError).exception).toBe(response);
        };

        it.each([
            [0, ErrorCodes.OFFLINE, 'You\'re offline, unable to communicate with servers. Try checking your internet connection.'],
            [503, ErrorCodes.OFFLINE, 'Apologies, servers can\'t be reached at this time. Try again in a few minutes.'],
            [500, ErrorCodes.NETWORK_EXCEPTION, 'Apologies, servers were unable to complete that request due to an internal error. Please contact support if this error still persists.'],
            [401, ErrorCodes.MISSING_PERMISSIONS, 'You do not have the correct permissions to complete this request. Please contact your local administrator for more information.']
        ])('maps HTTP status %s to a user-facing error', (status, code, message) => {
            expectMappedError(networkError(status as number), code as ErrorCodes, message as string);
        });

        it('uses the API message for invalid requests', () => {
            expectMappedError(
                networkError(400, { UserMessage: 'The payload is invalid.' }),
                ErrorCodes.INVALID_ACTION,
                'The payload is invalid.'
            );
        });

        it('uses a fallback message when an invalid request has no API message', () => {
            expectMappedError(
                networkError(406, {}),
                ErrorCodes.INVALID_ACTION,
                'Failed to perform action, there seems to be an error in your request. Please check your request is valid.'
            );
        });

        it('maps unknown HTTP statuses to a network error', () => {
            const response = networkError(418);
            expectMappedError(response, ErrorCodes.NETWORK_EXCEPTION, 'Network error [418] - Http failure response for (unknown url): 418 failure');
        });

        it('passes non-HTTP errors through unchanged', () => {
            const error = new Error('unexpected');
            let actual: unknown;

            throwError(() => error).pipe(mapNetworkError())
                .subscribe({ error: (value) => actual = value });

            expect(actual).toBe(error);
        });
    });
});
