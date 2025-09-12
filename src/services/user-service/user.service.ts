import { Injectable } from '@angular/core';
import { UserRepository } from './user.repository';
import { Observable, of, tap } from 'rxjs';
import { IUser } from './types/user.type';

@Injectable()
export class UserService {

    private _repository: UserRepository;
    private _user: IUser | null = null

    constructor(repository: UserRepository) {
        this._repository = repository;
    }

    public getUserBySession(): Observable<IUser> {

        if (this._user !== null)
            return of(this._user);

        return this._repository.getUserBySession()
            .pipe(tap((user) => {
                this._user = user;
            }));
    }

    public getUser(userReference: string): Observable<IUser> {
        return this._repository.getUser(userReference);
    }

    public removeUser(): void {
        this._user = null;
    }
}
