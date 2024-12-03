import { Component, inject, ViewChild } from '@angular/core';
import { EmployeeDetailComponent } from '../employee-detail/employee-detail.component';
import { EmployeesClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { EmployeeSubordinateTreeComponent } from '../employee-subordinate-tree/employee-subordinate-tree.component';
import { DataGridComponent } from 'src/app/shared/components/data-grid/data-grid.component';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.scss',
  providers: [EmployeesClient]
})
export class EmployeeListComponent {
  detailComponent = EmployeeDetailComponent;
  pageId = 'b6ed8006-420d-43e0-6e40-08dd0659aaf7'

  @ViewChild('grid') grid: DataGridComponent;

  entityClient: EmployeesClient = inject(EmployeesClient);
  customDialogService: CustomDialogService = inject(CustomDialogService);

  onHandleRowAction(event){
    if (event.action.actionName === 'subordinate') {
      this.openSubordinateTree(event);
    }
  }

  private openSubordinateTree(event: any) {
    this.customDialogService.handleCloseIcon = false;
    const paymentListDialogRef = this.customDialogService.openDialog<string>(
      EmployeeSubordinateTreeComponent,
      event.data.id,
      `Subordinate of ${event.data.firstName} `,
      { width: '70vw' },
      // null,
      true
    );
    paymentListDialogRef.onClose.subscribe((succeeded) => {
      this.grid.refreshGrid();
    });

    // this.customDialogService.handelCloseIconClick.subscribe((succeeded) => {
    //   if(this.customDialogService?.handleCloseIcon){
    //     console.log('refresh grid')
    //     this.grid.refreshGrid();
    //   }
    // });
  }
}
