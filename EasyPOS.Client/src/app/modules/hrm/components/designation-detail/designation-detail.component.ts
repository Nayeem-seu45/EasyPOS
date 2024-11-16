import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { DesignationsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-designation-detail',
  templateUrl: './designation-detail.component.html',
  styleUrl: './designation-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: DesignationsClient}]
})
export class DesignationDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: DesignationsClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      name: [null],
      code: [null],
      description: [null],
      status: [null],
      departmentId: [null],
      parentId: [null]

    });
  }

}
