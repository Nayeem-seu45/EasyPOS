import { Component, inject } from '@angular/core';
import { LeaveTypeDetailComponent } from '../leave-type-detail/leave-type-detail.component';
import { LeaveTypesClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-leave-type-list',
  templateUrl: './leave-type-list.component.html',
  styleUrl: './leave-type-list.component.scss',
  providers: [LeaveTypesClient]
})
export class LeaveTypeListComponent {
  detailComponent = LeaveTypeDetailComponent;
  pageId = '29acb7af-1b7f-4f8a-d3cc-08dd06726ddc'

  entityClient: LeaveTypesClient = inject(LeaveTypesClient);
}
