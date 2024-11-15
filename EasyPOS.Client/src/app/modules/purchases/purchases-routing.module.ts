import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PurchaseDetailComponent } from "./components/purchase-detail/purchase-detail.component";
import { PurchaseInfoDetailComponent } from "./components/purchase-info-detail/purchase-info-detail.component";
import { PurchaseListComponent } from "./components/purchase-list/purchase-list.component";
import { UpdatePurchaseOrderDetailComponent } from "./components/update-purchase-order-detail/update-purchase-order-detail.component";
import { PurchasePaymentDetailComponent } from "./components/purchase-payment-detail/purchase-payment-detail.component";
import { PurchasePaymentListComponent } from "./components/purchase-payment-list/purchase-payment-list.component";
import { PurchaseReturnListComponent } from "./components/purchase-return-list/purchase-return-list.component";
import { PurchaseReturnDetailComponent } from "./components/purchase-return-detail/purchase-return-detail.component";

const routes: Routes = [
  {path: 'list', component: PurchaseListComponent},
  {path: 'create', component: PurchaseDetailComponent},
  {path: 'edit/:id', component: PurchaseDetailComponent},
  {path: 'detail/:id', component: PurchaseInfoDetailComponent},
  {path: 'return-list', component: PurchaseReturnListComponent},
  {path: 'return/create/:purchaseId', component: PurchaseReturnDetailComponent, data: {processFrom: 'purchase-list'}},
  {path: 'return/edit/:id', component: PurchaseReturnDetailComponent, data: {processFrom: 'return-list'}},
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class PurchaseRoutingModule{}

export const purchaseRoutingComponents = [
  PurchaseListComponent,
  PurchaseDetailComponent,
  PurchaseInfoDetailComponent,
  UpdatePurchaseOrderDetailComponent,
  PurchasePaymentDetailComponent,
  PurchasePaymentListComponent,
  PurchaseReturnListComponent,
  PurchaseReturnDetailComponent
]