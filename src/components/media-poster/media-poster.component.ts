import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import {IMedia} from "../../services/media-service/types/media.type";
import {MediaLink} from "../../services/media-service/media-link";

@Component({
    selector: 'media-poster',
    templateUrl: './media-poster.component.html',
    styleUrls: ['./media-poster.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class MediaPosterComponent {

    @Input()
    public media: IMedia | null = null;

    public linkFor(media: IMedia): Array<string> {
        return MediaLink.for(media);
    }

    public trimTitle(media: IMedia) : string {

        const maxLength = 15;
        let title = media.grandParentTitle ?? media.title ?? '';

        if (title.length > maxLength) {
        return `${title.substring(0, maxLength).trim()}...`;
        }

        return title;
    }
}
