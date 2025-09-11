export class Session {

    private _identifier: string;

    constructor(sessionId: string) {
        this._identifier = sessionId;
    }

    public identifier(): string {
        return this._identifier;
    }
}