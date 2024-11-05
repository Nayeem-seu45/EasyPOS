import { Component, inject } from '@angular/core';
import { CouponDetailComponent } from '../coupon-detail/coupon-detail.component';
import { CouponsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-coupon-list',
  templateUrl: './coupon-list.component.html',
  styleUrl: './coupon-list.component.scss',
  providers: [CouponsClient]
})
export class CouponListComponent {
  detailComponent = CouponDetailComponent;
  pageId = '48ca2306-927a-41f8-08c4-08dcf6b5837c'

  entityClient: CouponsClient = inject(CouponsClient);
}
