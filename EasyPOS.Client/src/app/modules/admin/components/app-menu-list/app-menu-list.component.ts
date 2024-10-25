import { Component } from '@angular/core';
import { AppMenuDetailComponent } from '../app-menu-detail/app-menu-detail.component';
import { AppMenusClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ReorderAppMenusComponent } from '../reorder-app-menus/reorder-app-menus.component';

@Component({
  selector: 'app-app-menu-list',
  templateUrl: './app-menu-list.component.html',
  styleUrl: './app-menu-list.component.scss',
  providers: [AppMenusClient]

})
export class AppMenuListComponent {
  detailComponent = AppMenuDetailComponent;
  pageId = '5255d7a0-49b8-45da-3f93-08dca9b2d959';

  constructor(public entityClient: AppMenusClient,
    private customDialogService: CustomDialogService,
  ){
    
  }

  onhandleToolbarAction(event){
    if(event.actionName === 'reorder'){
      const reorderDialog = this.customDialogService.openDialog(ReorderAppMenusComponent, null, 'Reorder App Menus');
    }
  }
}

