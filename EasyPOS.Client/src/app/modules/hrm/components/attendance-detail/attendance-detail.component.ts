import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { AttendancesClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-attendance-detail',
  templateUrl: './attendance-detail.component.html',
  styleUrl: './attendance-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: AttendancesClient}]
})
export class AttendanceDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: AttendancesClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      employeeId: [null],
      attendanceStatusId: [null]

    });
  }

}
