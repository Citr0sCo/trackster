import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, Subject, takeUntil } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { IMedia } from '../../services/media-service/types/media.type';

@Component({
    selector: 'search-page',
    templateUrl: './search-page.component.html',
    styleUrls: ['./search-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SearchPageComponent implements OnInit, OnDestroy {

    public readonly queryControl = new FormControl('', { nonNullable: true });

    public results: Array<IMedia> = [];
    public query: string = '';
    public isLoading: boolean = false;
    public hasSearched: boolean = false;

    private readonly _mediaService: MediaService;
    private readonly _router: Router;
    private readonly _activatedRoute: ActivatedRoute;
    private readonly _destroy: Subject<void> = new Subject();

    constructor(mediaService: MediaService, router: Router, activatedRoute: ActivatedRoute) {
        this._mediaService = mediaService;
        this._router = router;
        this._activatedRoute = activatedRoute;
    }

    public ngOnInit(): void {
        this._activatedRoute.queryParamMap
            .pipe(takeUntil(this._destroy))
            .subscribe((params) => {
                this.query = (params.get('query') ?? '').trim();
                this.queryControl.setValue(this.query, { emitEvent: false });
                this.search(this.query);
            });
    }

    public submit(): void {
        const query = this.queryControl.value.trim();

        if (!query) {
            return;
        }

        this._router.navigate(['/app/search'], { queryParams: { query } });
    }

    public ngOnDestroy(): void {
        this._destroy.next();
        this._destroy.complete();
    }

    private search(query: string): void {
        if (!query) {
            this.results = [];
            this.hasSearched = false;
            return;
        }

        this.isLoading = true;

        this._mediaService.search(query, 100)
            .pipe(
                takeUntil(this._destroy),
                finalize(() => this.isLoading = false)
            )
            .subscribe((results) => {
                this.results = results;
                this.isLoading = false;
                this.hasSearched = true;
            });
    }
}
