import { Component, OnInit } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';

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
export class CustomSidebarComponent implements OnInit {

    public showMenu: boolean = false;
    public isDesktop: boolean = false;

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
    }

    public toggleMenu(): void {

        if (this.isDesktop) {
            return;
        }


        this.showMenu = !this.showMenu;
    }

}
