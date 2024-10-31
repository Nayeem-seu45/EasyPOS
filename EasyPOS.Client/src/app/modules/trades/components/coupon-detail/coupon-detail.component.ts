import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { CouponsClient, DiscountType } from 'src/app/modules/generated-clients/api-service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-coupon-detail',
  templateUrl: './coupon-detail.component.html',
  styleUrl: './coupon-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: CouponsClient}]
})
export class CouponDetailComponent extends BaseDetailComponent {
  activeTabIndex = 0;
  discountTypesSelectList: { id: number, name: string }[] = [];

  constructor(@Inject(ENTITY_CLIENT) entityClient: CouponsClient){
    super(entityClient)

    this.discountTypesSelectList = CommonUtils.enumToArray(DiscountType);

  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      code: [null],
      name: [null],
      description: [null],
      discountType: [null],
      amount: [null],
      expiryDate: [null],
      allowFreeShipping: [null],
      minimumSpend: [null],
      maximumSpend: [null],
      onlyIndivisual: [null],
      perCouponUsageLimit: [null],
      perUserUsageLimit: [null]

    });
  }

}
