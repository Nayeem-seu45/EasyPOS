import { Injectable, Type } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SidebarService {
  private sidebarState = new Subject<{ component: Type<any>, data?: any, config?: any }>();
  sidebarState$ = this.sidebarState.asObservable();

  private sidebarClose = new Subject<any>();
  sidebarClose$ = this.sidebarClose.asObservable();

  open(component: Type<any>, data?: any, config?: any) {
    this.sidebarState.next({ component, data, config });
  }

  close(returnData?: any) {
    this.sidebarClose.next(returnData);
  }
}