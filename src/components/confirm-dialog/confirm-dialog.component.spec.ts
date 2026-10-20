import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConfirmDialogComponent } from './confirm-dialog.component';

describe('ConfirmDialogComponent', () => {
    let fixture: ComponentFixture<ConfirmDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ConfirmDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConfirmDialogComponent);
        fixture.componentInstance.title = 'Remove from watched history?';
        fixture.componentInstance.message = 'This cannot be undone.';
    });

    it('renders only while open', () => {
        fixture.detectChanges();
        expect(fixture.nativeElement.querySelector('.confirm-dialog')).toBeNull();

        fixture.componentInstance.isOpen = true;
        fixture.detectChanges();

        expect(fixture.nativeElement.querySelector('.confirm-dialog')).not.toBeNull();
        expect(fixture.nativeElement.querySelector('.confirm-dialog__title').textContent).toContain('Remove from watched history?');
    });

    it('emits confirmed and cancelled from the dialog actions', () => {
        const events: Array<string> = [];
        fixture.componentInstance.confirmed.subscribe(() => events.push('confirmed'));
        fixture.componentInstance.cancelled.subscribe(() => events.push('cancelled'));
        fixture.componentInstance.isOpen = true;
        fixture.detectChanges();

        (fixture.nativeElement.querySelector('.btn-danger') as HTMLButtonElement).click();
        (fixture.nativeElement.querySelector('.btn-quiet') as HTMLButtonElement).click();

        expect(events).toEqual(['confirmed', 'cancelled']);
    });

    it('ignores actions while busy', () => {
        const events: Array<string> = [];
        fixture.componentInstance.confirmed.subscribe(() => events.push('confirmed'));
        fixture.componentInstance.cancelled.subscribe(() => events.push('cancelled'));
        fixture.componentInstance.isOpen = true;
        fixture.componentInstance.isBusy = true;
        fixture.detectChanges();

        (fixture.nativeElement.querySelector('.btn-danger') as HTMLButtonElement).click();
        (fixture.nativeElement.querySelector('.btn-quiet') as HTMLButtonElement).click();

        expect(events).toEqual([]);
    });

    it('cancels on escape unless busy', () => {
        const events: Array<string> = [];
        fixture.componentInstance.cancelled.subscribe(() => events.push('cancelled'));
        fixture.componentInstance.isOpen = true;
        fixture.detectChanges();

        document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape' }));
        expect(events).toEqual(['cancelled']);

        fixture.componentInstance.isBusy = true;
        fixture.detectChanges();
        document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape' }));
        expect(events).toEqual(['cancelled']);
    });

    it('cancels when the backdrop is clicked', () => {
        const events: Array<string> = [];
        fixture.componentInstance.cancelled.subscribe(() => events.push('cancelled'));
        fixture.componentInstance.isOpen = true;
        fixture.detectChanges();

        (fixture.nativeElement.querySelector('.confirm-dialog') as HTMLElement).click();
        expect(events).toEqual(['cancelled']);

        (fixture.nativeElement.querySelector('.confirm-dialog__panel') as HTMLElement).click();
        expect(events).toEqual(['cancelled']);
    });
});
