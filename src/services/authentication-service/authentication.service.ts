import {Injectable} from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationRepository } from './authentication.repository';
import {ISignInRequest} from "./types/sign-in.request";
import {ISignInResponse} from "./types/sign-in.response";

@Injectable()
export class AuthenticationService {

    private _authenticationRepository: AuthenticationRepository;

    constructor(authenticationRepository: AuthenticationRepository) {
        this._authenticationRepository = authenticationRepository;
    }

    public signIn(provider: number, payload: ISignInRequest): Observable<ISignInResponse> {
        return this._authenticationRepository.signIn({
            Provider: provider,
            Code: payload.code
        });
    }
}
