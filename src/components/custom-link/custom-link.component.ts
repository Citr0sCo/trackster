import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ILink } from '../../services/link-service/types/link.type';
import { LinkService } from '../../services/link-service/link.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { IStatModel } from '../../services/stats-service/types/stat-model.type';
import { environment } from '../../environments/environment';

@Component({
    selector: 'custom-link',
    templateUrl: './custom-link.component.html',
    styleUrls: ['./custom-link.component.scss'],
    standalone: false
})
export class CustomLinkComponent implements OnInit, OnDestroy {

    @Input()
    public item: ILink | null = null;

    @Input()
    public stats: Array<IStatModel> = new Array<IStatModel>();

    @Input()
    public isEditModeEnabled: boolean = false;

    @Input()
    public showWidgets: boolean = false;

    @Output()
    public updated: EventEmitter<void> = new EventEmitter<void>();

    public isDeleting: boolean = false;
    public isEditing: boolean = false;
    public isLoading: boolean = false;
    public isDeleted: boolean = false;
    public logoUpdated: boolean = false;
    public successMessage: string | null = null;
    public errorMessage: string | null = null;
    public showIcon: boolean = true;

    public form: FormGroup = new FormGroup<any>({
        name: new FormControl('', Validators.required),
        url: new FormControl('', Validators.required),
        host: new FormControl('', Validators.required),
        port: new FormControl('', Validators.required),
        isSecure: new FormControl('', Validators.required),
        iconUrl: new FormControl('', Validators.required)
    });

    private readonly _linkService: LinkService;
    private readonly _destroy: Subject<void> = new Subject();

    constructor(linkService: LinkService) {
        this._linkService = linkService;
    }

    public ngOnInit(): void {
        this.form = new FormGroup<any>({
            name: new FormControl(this.item!.name, Validators.required),
            url: new FormControl(this.item!.url, Validators.required),
            host: new FormControl(this.item!.host, Validators.required),
            port: new FormControl(this.item!.port, Validators.required),
            isSecure: new FormControl(this.item!.isSecure, Validators.required),
            iconUrl: new FormControl(this.item!.iconUrl, Validators.required)
        });

        this.item!.iconUrl = environment.apiBaseUrl + '/api/files/' + this.item!.identifier;
    }

    public deleteLink(): void {
        this.isLoading = true;

        this._linkService.deleteLink(this.item!.identifier!)
            .pipe(takeUntil(this._destroy))
            .subscribe(() => {
                this.isLoading = false;
                this.isDeleted = true;
            });
    }

    public updateLink(): void {
        this.isLoading = true;

        this._linkService.updateLink({
            identifier: this.item!.identifier,
            containerName: this.item!.containerName,
            name: this.form.get('name')!.value,
            url: this.form.get('url')!.value,
            isSecure: this.form.get('isSecure')!.value,
            host: this.form.get('host')!.value,
            port: this.form.get('port')!.value,
            category: this.item!.category,
            sortOrder: this.item!.sortOrder,
            iconUrl: this.form.get('iconUrl')!.value
        })
            .pipe(takeUntil(this._destroy))
            .subscribe((link) => {
                this.isLoading = false;
                this.item = link;
                this.successMessage = 'Successfully updated link.';
            });
    }

    public moveUp(): void {
        this._linkService.updateLink({
            identifier: this.item!.identifier,
            containerName: this.item!.containerName,
            name: this.form.get('name')!.value,
            url: this.form.get('url')!.value,
            isSecure: this.form.get('isSecure')!.value,
            host: this.form.get('host')!.value,
            port: this.form.get('port')!.value,
            category: this.item!.category,
            sortOrder: this.item!.sortOrder,
            iconUrl: this.form.get('iconUrl')!.value
        }, true, false)
            .pipe(takeUntil(this._destroy))
            .subscribe(() => {
                this.updated.emit();
            });
    }

    public moveDown(): void {
        this._linkService.updateLink({
            identifier: this.item!.identifier,
            containerName: this.item!.containerName,
            name: this.form.get('name')!.value,
            url: this.form.get('url')!.value,
            isSecure: this.form.get('isSecure')!.value,
            host: this.form.get('host')!.value,
            port: this.form.get('port')!.value,
            category: this.item!.category,
            sortOrder: this.item!.sortOrder,
            iconUrl: this.form.get('iconUrl')!.value
        }, false, true)
            .pipe(takeUntil(this._destroy))
            .subscribe(() => {
                this.updated.emit();
            });
    }

    public handleFileUpload(e: any): void {

        if (e.target.files.length === 0) {
            return;
        }

        this.isLoading = true;

        const file = e.target.files[0] as File;

        const fileReader = new FileReader();
        fileReader.readAsArrayBuffer(file);

        fileReader.onload = () => {
            const arrayBuffer = fileReader.result as ArrayBuffer;
            const blob = new Blob([arrayBuffer], { type: file.type });

            const formData = new FormData();
            formData.append('Logo', blob, file.name);

            this.showIcon = false;
            this._linkService.uploadLogo(this.item!.identifier!, formData)
                .pipe(takeUntil(this._destroy))
                .subscribe((logoUrl: string) => {
                    this.isLoading = false;
                    this.logoUpdated = true;
                    this.showIcon = true;
                });
        };
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }

    protected readonly environment = environment;

    public handleIconError(): void {

        if(this.item!.iconUrl.indexOf('https://cdn.jsdelivr.net/') === -1) {
            this.item!.iconUrl = `https://cdn.jsdelivr.net/gh/selfhst/icons/png/${this.item!.name.replace(' ', '-').toLowerCase()}.png`;
            return;
        }

        this.item!.iconUrl = './assets/apps/default.png';
    }
}
