import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ProductTransferDetailComponent } from "./components/product-transfer-detail/product-transfer-detail.component";
import { ProductTransferInfoDetailComponent } from "./components/product-transfer-info-detail/product-transfer-info-detail.component";
import { ProductTransferListComponent } from "./components/product-transfer-list/product-transfer-list.component";
import { UpdateProductTransferOrderDetailComponent } from "./components/update-product-transfer-order-detail/update-product-transfer-order-detail.component";

const routes: Routes = [
  
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})

export class ProductTransferRoutingModule{}

export const productTransferRoutingComponents = [
  ProductTransferDetailComponent
  , ProductTransferListComponent
  , UpdateProductTransferOrderDetailComponent
  , ProductTransferInfoDetailComponent
]