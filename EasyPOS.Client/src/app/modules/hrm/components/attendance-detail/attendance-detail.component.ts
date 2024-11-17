import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { AttendancesClient } from 'src/app/modules/generated-clients/api-service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-attendance-detail',
  templateUrl: './attendance-detail.component.html',
  styleUrl: './attendance-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: AttendancesClient}, DatePipe]
})
export class AttendanceDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: AttendancesClient,
  private datePipe: DatePipe){
    super(entityClient)
  }

  override beforeActionProcess(command: any): any {
    return {
      ...command,
      attendanceDate: this.datePipe.transform(command.attendanceDate, 'yyyy-MM-dd'),
      // checkIn: this.datePipe.transform(command.checkIn, 'HH:mm:ss'),
      // checkOut: this.datePipe.transform(command.checkOut, 'HH:mm:ss'),
    };
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      employeeId: [null],
      attendanceDate: [null],
      checkIn: [null],
      checkOut: [null],
      statusId: [null]
    });
  }

}
