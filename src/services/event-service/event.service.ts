import {Subject} from "rxjs";

export class EventService {
    public scrolledToBottom: Subject<boolean> = new Subject();

    public scrolledToBottomOfThePage() {
        this.scrolledToBottom.next(true);
    }

    public notScrolledToBottomOfThePage() {
        this.scrolledToBottom.next(false);
    }
}