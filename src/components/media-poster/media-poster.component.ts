import { Component, Input } from '@angular/core';

@Component({
    selector: 'media-poster',
    templateUrl: './media-poster.component.html',
    styleUrls: ['./media-poster.component.scss'],
    standalone: false
})
export class MediaPosterComponent {

    @Input()
    public title: string = '';

    @Input()
    public icon: string = '';

    @Input()
    public disabled: boolean = false;

    @Input()
    public isLoading: boolean = false;
}
