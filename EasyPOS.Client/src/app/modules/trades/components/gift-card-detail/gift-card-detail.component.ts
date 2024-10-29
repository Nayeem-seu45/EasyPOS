import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { GiftCardsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-gift-card-detail',
  templateUrl: './gift-card-detail.component.html',
  styleUrl: './gift-card-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: GiftCardsClient}]
})
export class GiftCardDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: GiftCardsClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      cardNo: [null],
      amount: [null],
      expense: [null],
      balance: [null],
      expiredDate: [null],
      customerId: [null],
      allowMultipleTransac: [false],
      giftCardType: [null],
      status: [null]

    });
  }

}
