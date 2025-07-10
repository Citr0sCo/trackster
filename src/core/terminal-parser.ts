export class TerminalParser {

    private readonly _terminalOutput: string;

    constructor(terminalOutput: string) {
        this._terminalOutput = terminalOutput;
    }

    public toHtml(): string {

        let html = this._terminalOutput;

        if (!html) {
            return '';
        }

        html = this._terminalOutput
            .replaceAll('\u001b[0;36m', '<span class="text-cyan">')
            .replaceAll('\u001b[0;32m', '<span class="text-green">')
            .replaceAll('\u001b[0m', '</span>')
            .replaceAll('\\n', '<br />')
            .replaceAll('\\"', '"')
            .replaceAll('\\\\"', '"');

        return html;
    }
}