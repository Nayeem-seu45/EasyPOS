import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { HolidaysClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-holiday-detail',
  templateUrl: './holiday-detail.component.html',
  styleUrl: './holiday-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: HolidaysClient}]
})
export class HolidayDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: HolidaysClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      title: [null],
      description: [null],
      isActive: [null]

    });
  }

}
