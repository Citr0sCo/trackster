import { ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { MediaService } from '../../services/media-service/media.service';
import { GlobalSearchComponent } from './global-search.component';

describe('GlobalSearchComponent', () => {
    const createComponent = () => {
        const router = { navigate: vi.fn() } as unknown as Router;
        const mediaService = { search: vi.fn(() => of([])) } as unknown as MediaService;
        const elementRef = new ElementRef(document.createElement('div'));

        return { component: new GlobalSearchComponent(mediaService, router, elementRef), router };
    };

    it('closes the results and notifies listeners when a result is picked', () => {
        const { component } = createComponent();
        let navigations = 0;
        component.navigated.subscribe(() => navigations++);
        component.isOpen = true;

        component.select();

        expect(component.isOpen).toBe(false);
        expect(navigations).toBe(1);
    });

    it('submits the trimmed query and ignores blank ones', () => {
        const { component, router } = createComponent();
        let navigations = 0;
        component.navigated.subscribe(() => navigations++);

        component.queryControl.setValue('   ');
        component.submit();

        expect(router.navigate).not.toHaveBeenCalled();

        component.queryControl.setValue(' matrix ');
        component.submit();

        expect(router.navigate).toHaveBeenCalledWith(['/app/search'], { queryParams: { query: 'matrix' } });
        expect(navigations).toBe(1);
    });
});
