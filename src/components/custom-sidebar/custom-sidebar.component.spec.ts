import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { By } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { provideRouter, RouterModule } from '@angular/router';
import { of } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { UserService } from '../../services/user-service/user.service';
import { GlobalSearchComponent } from '../global-search/global-search.component';
import { CustomSidebarComponent } from './custom-sidebar.component';

describe('CustomSidebarComponent', () => {
    const createComponent = () => new CustomSidebarComponent({
        getUserBySession: () => of(null)
    } as unknown as UserService);

    it('closes the menu after navigating on mobile', () => {
        const component = createComponent();
        component.showMenu = true;
        component.isDesktop = false;

        component.closeMenu();

        expect(component.showMenu).toBe(false);
    });

    it('keeps the menu visible on desktop', () => {
        const component = createComponent();
        component.showMenu = true;
        component.isDesktop = true;

        component.closeMenu();

        expect(component.showMenu).toBe(true);
    });
});

describe('CustomSidebarComponent search wiring', () => {
    let fixture: ComponentFixture<CustomSidebarComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CustomSidebarComponent, GlobalSearchComponent],
            imports: [RouterModule, ReactiveFormsModule],
            providers: [
                provideRouter([]),
                provideNoopAnimations(),
                { provide: UserService, useValue: { getUserBySession: () => of(null) } },
                { provide: MediaService, useValue: { search: () => of([]) } }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(CustomSidebarComponent);

        fixture.detectChanges();

        // ngOnInit derives the layout from window.innerWidth, so force the mobile state afterwards.
        fixture.componentInstance.isDesktop = false;
        fixture.componentInstance.showMenu = true;
        fixture.detectChanges();
    });

    it('closes the menu when the search result navigates', () => {
        const search = fixture.debugElement.query(By.directive(GlobalSearchComponent))
            .componentInstance as GlobalSearchComponent;

        search.select();
        fixture.detectChanges();

        expect(fixture.componentInstance.showMenu).toBe(false);
    });
});
