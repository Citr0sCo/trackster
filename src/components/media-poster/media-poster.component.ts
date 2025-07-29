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

    public trimTitle(media: IMedia) : string {

        const maxLength = 15;
        let title = media.grandParentTitle ?? media.title ?? '';

        if (title.length > maxLength) {
        return `${title.substring(0, maxLength).trim()}...`;
        }

        return title;
    }
}
