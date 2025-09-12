import { Injectable } from '@angular/core';
import { Observable, pipe, tap } from 'rxjs';
import { AuthenticationRepository } from './authentication.repository';
import { ISignInRequest } from "./types/sign-in.request";
import { ISignInResponse } from "./types/sign-in.response";
import { Session } from "../../core/session";
import { IRegisterRequest } from "./types/register.request";
import { IRegisterResponse } from "./types/register.response";
import { UserService } from '../user-service/user.service';

@Injectable()
export class AuthenticationService {

    private _authenticationRepository: AuthenticationRepository;
    private _session: Session | null = null;
    private _userService: UserService;

    constructor(authenticationRepository: AuthenticationRepository, userService: UserService) {
        this._authenticationRepository = authenticationRepository;
        this._userService = userService;

        const storedSessionId = localStorage.getItem('TRACKSTER_SESSION_ID');

        if (storedSessionId) {
            this._session = new Session(storedSessionId);
        }
    }

    public signIn(provider: number, payload: ISignInRequest): Observable<ISignInResponse> {
        return this._authenticationRepository.signIn({
            Provider: provider,
            Code: payload.code,
            Email: payload.email,
            Password: payload.password
        }).pipe(tap((response: ISignInResponse) => {
            this._session = new Session(response.sessionId);
            localStorage.setItem('TRACKSTER_SESSION_ID', this._session.identifier());
        }));
    }

    public logout(): Observable<ISignInResponse> {

        let session: string = this._session?.identifier() ?? '';
        if (!session) {
            session = localStorage.getItem('TRACKSTER_SESSION_ID') ?? '';
        }

        return this._authenticationRepository.signOut(session)
            .pipe(tap((response: ISignInResponse) => {
                this._session = new Session(response.sessionId);
                localStorage.removeItem('TRACKSTER_SESSION_ID');
                this._userService.removeUser();
            }));
    }

    public getAuthToken(): string | null | undefined {
        return localStorage.getItem('TRACKSTER_SESSION_ID');
    }

    public register(provider: number, payload: IRegisterRequest): Observable<IRegisterResponse> {
        return this._authenticationRepository.register({
            Provider: provider,
            Username: payload.username,
            Email: payload.email,
            Password: payload.password
        }).pipe(tap((response: IRegisterResponse) => {
            this._session = new Session(response.sessionId);
            localStorage.setItem('TRACKSTER_SESSION_ID', this._session.identifier());
        }));
    }

    public isLoggedIn(): boolean {
        return !!this._session?.identifier();
    }
}
