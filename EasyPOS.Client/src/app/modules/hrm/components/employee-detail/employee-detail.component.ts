import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { EmployeesClient } from 'src/app/modules/generated-clients/api-service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-employee-detail',
  templateUrl: './employee-detail.component.html',
  styleUrl: './employee-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: EmployeesClient}, DatePipe]
})
export class EmployeeDetailComponent extends BaseDetailComponent {

  activeTabIndex = 0;

  constructor(@Inject(ENTITY_CLIENT) entityClient: EmployeesClient,
  private datePipe: DatePipe){
    super(entityClient)
  }

  override beforeActionProcess(command: any): any {
    return {
      ...command,
      dob: this.datePipe.transform(command.dob, 'yyyy-MM-dd')
    };
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      code: [null],
      firstName: [null],
      lastName: [null],
      gender: [null],
      nid: [null],
      dob: [null],
      warehouseId: [null],
      departmentId: [null],
      designationId: [null],
      workingShiftId: [null],
      reportTo: [null],
      email: [null],
      phoneNo: [null],
      mobileNo: [null],
      country: [null],
      city: [null],
      address: [null],
      leaveTypes: [null]

    });
  }

}
