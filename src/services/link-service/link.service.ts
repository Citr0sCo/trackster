import { Injectable } from '@angular/core';
import { ILink } from './types/link.type';
import { Observable, of, Subject, tap } from 'rxjs';
import { LinkRepository } from './link.repository';

@Injectable()
export class LinkService {

    private _linkRepository: LinkRepository;
    private _cachedLinks: Array<ILink> | null = null;

    constructor(linkRepository: LinkRepository) {
        this._linkRepository = linkRepository;
    }

    public getUpdatedLinks(): Observable<Array<ILink>> {
        return this._linkRepository.getAllLinks()
            .pipe(
                tap((links) => {
                    this._cachedLinks = links;
                    localStorage.setItem('cachedLinks', JSON.stringify(links));
                })
            );
    }

    public getAllLinks(): Observable<Array<ILink>> {

        if (localStorage.getItem('cachedLinks')) {
            this._cachedLinks = JSON.parse(`${localStorage.getItem('cachedLinks')}`);
        }

        if (this._cachedLinks !== null) {
            return of(this._cachedLinks);
        }

        return this.getUpdatedLinks();
    }

    public addLink(link: ILink): Observable<ILink> {
        return this._linkRepository.addLink(link);
    }

    public importLinks(links: Array<ILink>): Observable<Array<ILink>> {
        return this._linkRepository.importLinks(links);
    }

    public updateLink(link: ILink, moveUp: boolean = false, moveDown: boolean = false): Observable<ILink> {
        return this._linkRepository.updateLink(link, moveUp, moveDown);
    }

    public deleteLink(identifier: string): Observable<any> {
        return this._linkRepository.deleteLink(identifier);
    }

    public uploadLogo(identifier: string, data: FormData): Observable<string> {
        return this._linkRepository.uploadLogo(identifier, data);
    }

    public getMediaLinks(): Observable<Array<ILink>> {
        return of(this._cachedLinks!.filter((link) => link.category === 'media') ?? []);
    }

    public getSystemLinks(): Observable<Array<ILink>> {
        return of(this._cachedLinks!.filter((link) => link.category === 'system') ?? []);
    }

    public getProductivityLinks(): Observable<Array<ILink>> {
        return of(this._cachedLinks!.filter((link) => link.category === 'productivity') ?? []);
    }

    public getToolsLinks(): Observable<Array<ILink>> {
        return of(this._cachedLinks!.filter((link) => link.category === 'tools') ?? []);
    }

    public createColumn(): Observable<void> {
        return this._linkRepository.createColumn();
    }

    public refreshCache(): Observable<void> {
        return this._linkRepository.refreshCache();
    }
}
