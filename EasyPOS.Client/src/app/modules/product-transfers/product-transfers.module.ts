import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppSharedModule } from 'src/app/shared/app-shared.module';
import { productTransferRoutingComponents, ProductTransferRoutingModule } from './product-transfers-routing.module';



@NgModule({
  declarations: [
    ...productTransferRoutingComponents
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,
    ProductTransferRoutingModule
  ]
})
export class ProductTransfersModule { }
