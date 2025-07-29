import { Component, Input } from '@angular/core';
import {IMedia} from "../../services/media-service/types/media.type";

@Component({
    selector: 'media-poster',
    templateUrl: './media-poster.component.html',
    styleUrls: ['./media-poster.component.scss'],
    standalone: false
})
export class MediaPosterComponent {

    @Input()
    public media: IMedia | null = null;
}
