import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HRMRoutingComponents, HRMRoutingModule } from './hrm-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppSharedModule } from 'src/app/shared/app-shared.module';
import { TabViewModule } from 'primeng/tabview';


@NgModule({
  declarations: [
    ...HRMRoutingComponents
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HRMRoutingModule,
    AppSharedModule,
    TabViewModule
  ]
})
export class HRMModule { }
