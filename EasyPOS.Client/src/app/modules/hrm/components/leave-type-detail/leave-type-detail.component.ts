import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { LeaveTypesClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-leave-type-detail',
  templateUrl: './leave-type-detail.component.html',
  styleUrl: './leave-type-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: LeaveTypesClient}]
})
export class LeaveTypeDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: LeaveTypesClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      name: [null],
      code: [null],
      totalLeaveDays: [null],
      maxConsecutiveDays: [null],
      isSandwichAllowed: [false]

    });
  }

}
