import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TradesRoutingComponents, TradesRoutingModule } from './trades-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppSharedModule } from 'src/app/shared/app-shared.module';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';


@NgModule({
  declarations: [
    ...TradesRoutingComponents
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,
    TradesRoutingModule,
    TableModule,
    TabViewModule  
  ]
})
export class TradesModule { }
