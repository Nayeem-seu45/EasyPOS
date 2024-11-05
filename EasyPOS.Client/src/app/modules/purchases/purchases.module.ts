import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppSharedModule } from 'src/app/shared/app-shared.module';
import { purchaseRoutingComponents, PurchaseRoutingModule } from './purchases-routing.module';



@NgModule({
  declarations: [
    ...purchaseRoutingComponents
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,
    PurchaseRoutingModule
  ]
})
export class PurchasesModule { }
