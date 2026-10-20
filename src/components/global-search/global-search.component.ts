import { ChangeDetectionStrategy, Component, ElementRef, HostListener, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, debounceTime, distinctUntilChanged, filter, map, of, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { MediaLink } from '../../services/media-service/media-link';
import { IMedia } from '../../services/media-service/types/media.type';

@Component({
    selector: 'global-search',
    templateUrl: './global-search.component.html',
    styleUrls: ['./global-search.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class GlobalSearchComponent implements OnInit, OnDestroy {

    public readonly queryControl = new FormControl('', { nonNullable: true });

    public results: Array<IMedia> = [];
    public isSearching: boolean = false;
    public hasSearched: boolean = false;
    public isOpen: boolean = false;

    private readonly _mediaService: MediaService;
    private readonly _router: Router;
    private readonly _elementRef: ElementRef<HTMLElement>;
    private readonly _destroy: Subject<void> = new Subject();

    constructor(mediaService: MediaService, router: Router, elementRef: ElementRef<HTMLElement>) {
        this._mediaService = mediaService;
        this._router = router;
        this._elementRef = elementRef;
    }

    public ngOnInit(): void {
        this.queryControl.valueChanges
            .pipe(
                debounceTime(250),
                map((query) => query.trim()),
                distinctUntilChanged(),
                tap((query) => {
                    this.hasSearched = false;

                    if (query.length < 2) {
                        this.results = [];
                        this.isSearching = false;
                    }
                }),
                filter((query) => query.length >= 2),
                switchMap((query) => {
                    this.isSearching = true;

                    return this._mediaService.search(query, 8)
                        .pipe(catchError(() => of([])));
                }),
                takeUntil(this._destroy)
            )
            .subscribe((results) => {
                this.results = results;
                this.isSearching = false;
                this.hasSearched = true;
            });
    }

    public linkFor(media: IMedia): Array<string> {
        return MediaLink.for(media);
    }

    public submit(): void {
        const query = this.queryControl.value.trim();

        if (!query) {
            return;
        }

        this.close();
        this._router.navigate(['/app/search'], { queryParams: { query } });
    }

    public close(): void {
        this.isOpen = false;
    }

    @HostListener('document:click', ['$event'])
    public onDocumentClick(event: MouseEvent): void {
        if (this.isOpen && !this._elementRef.nativeElement.contains(event.target as Node)) {
            this.close();
        }
    }

    public ngOnDestroy(): void {
        this._destroy.next();
        this._destroy.complete();
    }
}
