import { Component, Inject } from '@angular/core';import { CommonConstants } from 'src/app/core/contants/common';
import { SuppliersClient } from 'src/app/modules/generated-clients/api-service';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';

@Component({
  selector: 'app-supplier-detail',
  templateUrl: './supplier-detail.component.html',
  styleUrl: './supplier-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: SuppliersClient}]
})
export class SupplierDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: SuppliersClient){
    super(entityClient)
  }

  get isNew(): boolean{
    return !this.id || this.id === CommonConstants.EmptyGuid
  }

  protected override initializeFormGroup(): void {
    this.form = this.fb.group({
      id: [null],
      name: [''],
      email: [''],
      phoneNo: [''],
      mobile: [''],
      country: [null],
      city: [''],
      address: [''],
      openingBalance: [0],
      isActive: [true],
    });
  }
}
