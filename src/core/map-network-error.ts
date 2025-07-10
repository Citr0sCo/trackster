import { catchError, OperatorFunction, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

export enum ErrorCodes {
    APP_EXCEPTION,
    NETWORK_EXCEPTION,
    MISSING_PERMISSIONS,
    INVALID_ACTION,
    OFFLINE
}

export class UserError implements Error {
    public message: string;
    public name: string = 'UserError';
    public stack: string | undefined;
    public code: ErrorCodes;
    public exception: Error;

    constructor(message: string, code: ErrorCodes, error?: Error) {
        this.message = message;
        this.code = code;
        this.exception = error ?? new Error(message);
        this.stack = this.exception.stack;
    }

    public toString(): string {
        return this.message;
    }
}

// @ts-ignore
export function mapNetworkError<T>(): OperatorFunction<T, T> {
    return catchError((data) => {
        if (data instanceof HttpErrorResponse) {
            switch (data.status) {
                case 0:
                    return throwError(() => new UserError('You\'re offline, unable to communicate with servers. Try checking your internet connection.', ErrorCodes.OFFLINE, data));
                case 503:
                    return throwError(() => new UserError('Apologies, servers can\'t be reached at this time. Try again in a few minutes.', ErrorCodes.OFFLINE, data));
                case 500:
                    return throwError(() => new UserError('Apologies, servers were unable to complete that request due to an internal error. Please contact support if this error still persists.', ErrorCodes.NETWORK_EXCEPTION, data));
                case 401:
                    return throwError(() => new UserError('You do not have the correct permissions to complete this request. Please contact your local administrator for more information.', ErrorCodes.MISSING_PERMISSIONS, data));
                case 400:
                case 406:
                    return throwError(() => new UserError(data.error.UserMessage || 'Failed to perform action, there seems to be an error in your request. Please check your request is valid.', ErrorCodes.INVALID_ACTION, data));
            }
            return throwError(() => new UserError(`Network error [${data.status}] - ${data.message}`, ErrorCodes.NETWORK_EXCEPTION, data));
        } else {
            return throwError(() => data);
        }
    });
}
