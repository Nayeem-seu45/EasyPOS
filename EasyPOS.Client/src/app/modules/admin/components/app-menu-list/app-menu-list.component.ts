import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppMenuDetailComponent } from '../app-menu-detail/app-menu-detail.component';
import { AppMenusClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ReorderAppMenusComponent } from '../reorder-app-menus/reorder-app-menus.component';
import { SidebarService } from 'src/app/shared/services/sidebar.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-app-menu-list',
  templateUrl: './app-menu-list.component.html',
  styleUrl: './app-menu-list.component.scss',
  providers: [AppMenusClient]

})
export class AppMenuListComponent implements OnInit, OnDestroy{
  detailComponent = AppMenuDetailComponent;
  pageId = '5255d7a0-49b8-45da-3f93-08dca9b2d959';

  private sidebarCloseSubscription: Subscription;

  constructor(public entityClient: AppMenusClient,
    private sidebarService: SidebarService
  ){
    
  }

  ngOnInit() {
    this.sidebarCloseSubscription = this.sidebarService.sidebarClose$.subscribe((returnData) => {
      console.log(returnData)
    });
  }

  ngOnDestroy() {
    if (this.sidebarCloseSubscription) {
      this.sidebarCloseSubscription.unsubscribe();
    }
  }

  onhandleToolbarAction(event){
    if(event.actionName === 'reorder'){
      this.sidebarService.open(ReorderAppMenusComponent, null, {title: 'Reorder App Menus', titleIcon:'pi pi-search', position: 'right', modal: true });
    }
  }
}

