import { Injectable } from '@angular/core';
import { SettingsRepository } from './settings.repository';
import { Observable, of, tap } from 'rxjs';
import { ISettings } from './types/settings.type';

@Injectable()
export class SettingsService {

    private _repository: SettingsRepository;
    private _settings: ISettings | null = null

    constructor(repository: SettingsRepository) {
        this._repository = repository;
    }

    public getSettings(): Observable<ISettings> {

        if (this._settings !== null)
            return of(this._settings);

        return this._repository.getSettings()
            .pipe(tap((settings) => {
                this._settings = settings;
            }));
    }
}
