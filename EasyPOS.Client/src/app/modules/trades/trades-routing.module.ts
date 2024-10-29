import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PurchaseDetailComponent } from "./components/purchase-detail/purchase-detail.component";
import { PurchaseListComponent } from "./components/purchase-list/purchase-list.component";
import { SaleDetailComponent } from "./components/sale-detail/sale-detail.component";
import { SaleListComponent } from "./components/sale-list/sale-list.component";
import { UpdateSaleDetailComponent } from "./components/update-sale-detail/update-sale-detail.component";
import { UpdatePurchaseOrderDetailComponent } from "./components/update-purchase-order-detail/update-purchase-order-detail.component";
import { PurchasePaymentDetailComponent } from "./components/purchase-payment-detail/purchase-payment-detail.component";
import { PurchasePaymentListComponent } from "./components/purchase-payment-list/purchase-payment-list.component";
import { PurchaseInfoDetailComponent } from "./components/purchase-info-detail/purchase-info-detail.component";
import { SaleInfoDetailComponent } from "./components/sale-info-detail/sale-info-detail.component";
import { SalePaymentDetailComponent } from "./components/sale-payment-detail/sale-payment-detail.component";
import { SalePaymentListComponent } from "./components/sale-payment-list/sale-payment-list.component";
import { CourierDetailComponent } from "./components/courier-detail/courier-detail.component";
import { CourierListComponent } from "./components/courier-list/courier-list.component";
import { CouponDetailComponent } from "./components/coupon-detail/coupon-detail.component";
import { CouponListComponent } from "./components/coupon-list/coupon-list.component";
import { GiftCardDetailComponent } from "./components/gift-card-detail/gift-card-detail.component";
import { GiftCardListComponent } from "./components/gift-card-list/gift-card-list.component";

const routes: Routes = [
  {path: 'sales', component: SaleListComponent},
  {path: 'sale/:id', component: SaleDetailComponent},
  {path: 'add-sale', component: SaleDetailComponent},
  {path: 'sale-detail/:id', component: SaleInfoDetailComponent},
  {path: 'purchases', component: PurchaseListComponent},
  {path: 'purchase/:id', component: PurchaseDetailComponent},
  {path: 'add-purchase', component: PurchaseDetailComponent},
  {path: 'purchase-detail/:id', component: PurchaseInfoDetailComponent},
  {path: 'couriers', component: CourierListComponent},
  {path: 'coupons', component: CouponListComponent},
  {path: 'giftcards', component: GiftCardListComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TradesRoutingModule{}

export const TradesRoutingComponents = [
  SaleListComponent,
  SaleDetailComponent,
  SaleInfoDetailComponent,
  SalePaymentListComponent,
  SalePaymentDetailComponent,
  UpdateSaleDetailComponent,

  PurchaseListComponent,
  PurchaseDetailComponent,
  PurchaseInfoDetailComponent,
  UpdatePurchaseOrderDetailComponent,
  PurchasePaymentDetailComponent,
  PurchasePaymentListComponent,

  CourierListComponent,
  CourierDetailComponent,
  CouponListComponent,
  CouponDetailComponent,
  GiftCardListComponent,
  GiftCardDetailComponent,
];