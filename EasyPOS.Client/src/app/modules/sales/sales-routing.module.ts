import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CouponDetailComponent } from "./components/coupon-detail/coupon-detail.component";
import { CouponListComponent } from "./components/coupon-list/coupon-list.component";
import { CourierDetailComponent } from "./components/courier-detail/courier-detail.component";
import { CourierListComponent } from "./components/courier-list/courier-list.component";
import { GiftCardDetailComponent } from "./components/gift-card-detail/gift-card-detail.component";
import { GiftCardListComponent } from "./components/gift-card-list/gift-card-list.component";
import { SaleDetailComponent } from "./components/sale-detail/sale-detail.component";
import { SaleInfoDetailComponent } from "./components/sale-info-detail/sale-info-detail.component";
import { SaleListComponent } from "./components/sale-list/sale-list.component";
import { SalePaymentDetailComponent } from "./components/sale-payment-detail/sale-payment-detail.component";
import { SalePaymentListComponent } from "./components/sale-payment-list/sale-payment-list.component";
import { UpdateSaleDetailComponent } from "./components/update-sale-detail/update-sale-detail.component";
import { SaleReturnDetailComponent } from "./components/sale-return-detail/sale-return-detail.component";
import { SaleReturnListComponent } from "./components/sale-return-list/sale-return-list.component";
import { SaleReturnPaymentListComponent } from "./components/sale-return-payment-list/sale-return-payment-list.component";
import { SaleReturnPaymentDetailComponent } from "./components/sale-return-payment-detail/sale-return-payment-detail.component";


const routes: Routes = [
  {path: 'list', component: SaleListComponent},
  {path: 'edit/:id', component: SaleDetailComponent},
  {path: 'create', component: SaleDetailComponent},
  {path: 'detail/:id', component: SaleInfoDetailComponent},
  {path: 'return/create/:saleId', component: SaleReturnDetailComponent, data: {processFrom: 'sale-list'}},
  {path: 'return/list', component: SaleReturnListComponent, data: {processFrom: 'return-list'}},
  {path: 'return/edit/:id', component: SaleReturnDetailComponent, data: {processFrom: 'return-list'}},
  {path: 'couriers', component: CourierListComponent},
  {path: 'coupons', component: CouponListComponent},
  {path: 'gift-cards', component: GiftCardListComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SalesRoutingModule{}

export const salesRoutingComponents = [
  SaleListComponent,
  SaleDetailComponent,
  SaleInfoDetailComponent,
  SalePaymentListComponent,
  SalePaymentDetailComponent,
  UpdateSaleDetailComponent,
  SaleReturnListComponent,
  SaleReturnDetailComponent,
  CourierListComponent,
  CourierDetailComponent,
  CouponListComponent,
  CouponDetailComponent,
  GiftCardListComponent,
  GiftCardDetailComponent,
  SaleReturnPaymentListComponent,
  SaleReturnPaymentDetailComponent
];