﻿import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { MoneyTransfersClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-money-transfer-detail',
  templateUrl: './money-transfer-detail.component.html',
  styleUrl: './money-transfer-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: MoneyTransfersClient}]

})
export class MoneyTransferDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: MoneyTransfersClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      fromAccountId: [null],
      toAccountId: [null],
      amount: [null],
      referenceNo: [null]

    });
  }

}
