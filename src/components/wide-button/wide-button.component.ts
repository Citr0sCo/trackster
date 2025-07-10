import { Component, Input } from '@angular/core';

@Component({
    selector: 'wide-button',
    templateUrl: './wide-button.component.html',
    styleUrls: ['./wide-button.component.scss'],
    standalone: false
})
export class WideButtonComponent {

    @Input()
    public title: string = '';

    @Input()
    public icon: string = '';

    @Input()
    public disabled: boolean = false;

    @Input()
    public isLoading: boolean = false;
}
