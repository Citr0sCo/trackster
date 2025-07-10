import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { LinkMapper } from './link.mapper';
import { environment } from '../../environments/environment';
import { ILink } from './types/link.type';
import { mapNetworkError } from '../../core/map-network-error';

@Injectable()
export class LinkRepository {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getAllLinks(): Observable<any> {
        return this._httpClient.get(`${environment.apiBaseUrl}/api/links`)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return LinkMapper.map(response.Links);
                })
            );
    }

    public addLink(link: ILink): Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/links`, { Link: link })
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return LinkMapper.mapSingle(response.Link);
                })
            );
    }

    public importLinks(links: Array<ILink>): Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/links/import`, { Links: links })
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return LinkMapper.map(response.Links);
                })
            );
    }

    public updateLink(link: ILink, moveUp: boolean = false, moveDown: boolean = false): Observable<any> {
        return this._httpClient.patch(`${environment.apiBaseUrl}/api/links/${link.identifier}`, { Link: link, MoveUp: moveUp, MoveDown: moveDown })
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return LinkMapper.mapSingle(response.Link);
                })
            );
    }

    public deleteLink(identifier: string): Observable<any> {
        return this._httpClient.delete(`${environment.apiBaseUrl}/api/links/${identifier}`)
            .pipe(
                mapNetworkError()
            );
    }

    public uploadLogo(identifier: string, data: FormData): Observable<string> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/links/${identifier}/logo`, data)
            .pipe(
                mapNetworkError(),
                map((response: any) => {
                    return response.IconUrl;
                })
            );
    }

    public createColumn(): Observable<any> {
        return this._httpClient.post(`${environment.apiBaseUrl}/api/columns`, {})
            .pipe(
                mapNetworkError()
            );
    }

    public refreshCache(): Observable<any> {
        return this._httpClient.delete(`${environment.apiBaseUrl}/api/files/cache`, {})
            .pipe(
                mapNetworkError()
            );
    }
}