import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { CouriersClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-courier-detail',
  templateUrl: './courier-detail.component.html',
  styleUrl: './courier-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: CouriersClient}]
})
export class CourierDetailComponent extends BaseDetailComponent {
  
  constructor(@Inject(ENTITY_CLIENT) entityClient: CouriersClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      name: [null],
      phoneNo: [null],
      mobileNo: [null],
      email: [null],
      address: [null]
    });
  }

}
