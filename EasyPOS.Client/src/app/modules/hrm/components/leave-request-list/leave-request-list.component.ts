import { Component, inject } from '@angular/core';
import { LeaveRequestDetailComponent } from '../leave-request-detail/leave-request-detail.component';
import { LeaveRequestsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-leave-request-list',
  templateUrl: './leave-request-list.component.html',
  styleUrl: './leave-request-list.component.scss',
  providers: [LeaveRequestsClient]
})
export class LeaveRequestListComponent {
  detailComponent = LeaveRequestDetailComponent;
  pageId = '053af1a2-7804-420c-6e44-08dd0659aaf7'

  entityClient: LeaveRequestsClient = inject(LeaveRequestsClient);
}
