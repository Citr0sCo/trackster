import { ChangeDetectionStrategy, Component, EventEmitter, HostListener, Input, Output } from '@angular/core';

@Component({
    selector: 'confirm-dialog',
    templateUrl: './confirm-dialog.component.html',
    styleUrls: ['./confirm-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ConfirmDialogComponent {

    @Input()
    public isOpen: boolean = false;

    @Input()
    public title: string = 'Are you sure?';

    @Input()
    public message: string = '';

    @Input()
    public confirmLabel: string = 'Confirm';

    @Input()
    public cancelLabel: string = 'Cancel';

    @Input()
    public isBusy: boolean = false;

    @Output()
    public confirmed: EventEmitter<void> = new EventEmitter<void>();

    @Output()
    public cancelled: EventEmitter<void> = new EventEmitter<void>();

    @HostListener('document:keydown.escape')
    public onEscape(): void {
        this.cancel();
    }

    public confirm(): void {
        if (this.isBusy) {
            return;
        }

        this.confirmed.emit();
    }

    public cancel(): void {
        if (this.isBusy) {
            return;
        }

        this.cancelled.emit();
    }
}
