import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { AppSharedModule } from 'src/app/shared/app-shared.module';
import { salesRoutingComponents, SalesRoutingModule } from './sales-routing.module';



@NgModule({
  declarations: [
    ...salesRoutingComponents
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,
    SalesRoutingModule,
    TableModule,
    TabViewModule  
  ]
})
export class SalesModule { }
