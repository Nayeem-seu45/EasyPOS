import { Injectable, Type } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { SidebarComponent } from '../components/sidebar/sidebar.component';

@Injectable({
  providedIn: 'root'
})
export class SidebarService {
  private visibilityChange = new Subject<boolean>();
  private closeSidebarSubject = new Subject<any>();
  private sidebarComponentRef: SidebarComponent | null = null;

  openSidebar<T>(component: Type<T>, inputs?: Partial<T>): Observable<any> {
    this.visibilityChange.next(true);


    return this.closeSidebarSubject.asObservable();
  }

  closeSidebar(result?: any) {
    this.visibilityChange.next(false);
    this.closeSidebarSubject.next(result);
  }

  getVisibilityChange(): Observable<boolean> {
    return this.visibilityChange.asObservable();
  }
}
