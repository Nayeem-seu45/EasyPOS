import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { WorkingShiftsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-working-shift-detail',
  templateUrl: './working-shift-detail.component.html',
  styleUrl: './working-shift-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: WorkingShiftsClient}]
})
export class WorkingShiftDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: WorkingShiftsClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      shiftName: [null],
      description: [null],
      isActive: [null]

    });
  }

}
