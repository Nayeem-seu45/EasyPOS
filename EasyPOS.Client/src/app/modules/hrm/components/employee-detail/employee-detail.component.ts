import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { EmployeesClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-employee-detail',
  templateUrl: './employee-detail.component.html',
  styleUrl: './employee-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: EmployeesClient}]
})
export class EmployeeDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: EmployeesClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      employeeCode: [null],
      employeeName: [null],
      gender: [null],
      nid: [null],
      warehouseId: [null],
      departmentId: [null],
      designationId: [null],
      workingShiftId: [null],
      email: [null],
      phoneNo: [null],
      mobileNo: [null],
      country: [null],
      city: [null],
      address: [null]

    });
  }

}
