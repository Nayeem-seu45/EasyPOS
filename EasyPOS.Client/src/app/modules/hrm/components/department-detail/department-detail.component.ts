import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { DepartmentsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-department-detail',
  templateUrl: './department-detail.component.html',
  styleUrl: './department-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: DepartmentsClient}]
})
export class DepartmentDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: DepartmentsClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      name: [null],
      code: [null],
      description: [null],
      status: [null],
      parentId: [null],
      departmentHeadId: [null]

    });
  }

}
