import { Component, OnInit } from '@angular/core';
import { LinkService } from '../../services/link-service/link.service';

@Component({
    selector: 'import-links',
    templateUrl: './import-links.component.html',
    styleUrls: ['./import-links.component.scss'],
    standalone: false
})
export class ImportLinksComponent implements OnInit {

    public successMessage: string = '';
    public errorMessage: string = '';

    private _linkService: LinkService;
    private _fileUploadDialog: any;

    constructor(linkService: LinkService) {
        this._linkService = linkService;
    }

    public ngOnInit(): void {
        this._fileUploadDialog = document.querySelector('.import-links-action');
    }

    public handleClick(): void {
        if (this._fileUploadDialog) {
            this._fileUploadDialog.click();
        }
    }

    public handleFileUpload($event: any): void {
        const reader = new FileReader();
        reader.onload = (event: any) => {
            this.handleFileParse(event);
            this._fileUploadDialog.value = '';
        };
        reader.readAsText($event.target.files[0]);
    }

    public handleFileParse(event: any): void {
        this.successMessage = '';
        this.errorMessage = '';

        const result = event.target?.result;
        const links = JSON.parse(result?.toString() ?? '');

        this._linkService.importLinks(links)
            .subscribe((response) => {
                this.successMessage = 'Successfully imported links!';
            }, (error) => {
                this.errorMessage = 'Failed to import links!';
            });

    }
}
