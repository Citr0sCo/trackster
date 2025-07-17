import {AfterViewInit, Component, HostListener} from '@angular/core';
import {EventService} from "../services/event-service/event.service";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
})
export class AppComponent implements AfterViewInit {

    private _eventService: EventService;

    constructor(eventService: EventService) {
        this._eventService = eventService;
    }

    public ngAfterViewInit(): void {
        const element = document.querySelector('.main-content');
        element!.addEventListener('scroll', () => {
            if (element!.scrollHeight - element!.clientHeight <= element!.scrollTop + 10) {
                this._eventService.scrolledToBottomOfThePage();
            } else {
                this._eventService.notScrolledToBottomOfThePage();
            }
        });
    }
}
