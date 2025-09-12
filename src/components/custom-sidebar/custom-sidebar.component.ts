import { Component, OnDestroy, OnInit } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { Subject, takeUntil } from 'rxjs';
import { UserService } from '../../services/user-service/user.service';
import { IUser } from '../../services/user-service/types/user.type';

@Component({
    selector: 'custom-sidebar',
    templateUrl: './custom-sidebar.component.html',
    styleUrls: ['./custom-sidebar.component.scss'],
    standalone: false,
    animations: [
        trigger('slideInFromLeft', [
            state('hidden', style({
                transform: 'translateX(-100%)'
            })),
            state('visible', style({
                transform: 'translateX(0)'
            })),
            transition('hidden => visible', [
                animate('200ms 0ms ease-in-out')
            ]),
            transition('visible => hidden', [
                animate('200ms 0ms ease-in-out')
            ])
        ])
    ]
})
export class CustomSidebarComponent implements OnInit, OnDestroy {

    public showMenu: boolean = false;
    public isDesktop: boolean = false;
    public user: IUser | null = null;

    private readonly _destroy: Subject<void> = new Subject();
    private readonly _userService: UserService;

    constructor(userService: UserService) {
        this._userService = userService;
    }

    public ngOnInit(): void {
        if (window.innerWidth > 768) {
            this.showMenu = true;
            this.isDesktop = true;
        } else {
            this.isDesktop = false;
        }

        window.addEventListener('resize', () => {
            if (window.innerWidth > 768) {
                this.showMenu = true;
                this.isDesktop = true;
            } else {
                this.showMenu = false;
                this.isDesktop = false;
            }
        });

        this._userService.getUserBySession()
            .pipe(takeUntil(this._destroy))
            .subscribe((user) => {
                this.user = user;
            });
    }

    public toggleMenu(): void {

        if (this.isDesktop) {
            return;
        }


        this.showMenu = !this.showMenu;
    }

    public ngOnDestroy(): void {
        this._destroy.next();
    }
}
