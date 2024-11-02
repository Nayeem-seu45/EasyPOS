import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppSharedModule } from 'src/app/shared/app-shared.module';
import { quotationRoutingComponents, QuotationsRoutingModule } from './quotations-routing.module';



@NgModule({
  declarations: [
    ...quotationRoutingComponents
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,
    QuotationsRoutingModule
  ]
})
export class QuotationsModule { }
