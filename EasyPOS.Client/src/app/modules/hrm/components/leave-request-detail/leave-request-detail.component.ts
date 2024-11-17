import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { LeaveRequestsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-leave-request-detail',
  templateUrl: './leave-request-detail.component.html',
  styleUrl: './leave-request-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: LeaveRequestsClient}]
})
export class LeaveRequestDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: LeaveRequestsClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      employeeId: [null],
      leaveTypeId: [null],
      startDate: [null],
      endDate: [null],
      totalDays: [null],
      statusId: [null],
      attachmentUrl: [null],
      reason: [null]
    });
  }

}
