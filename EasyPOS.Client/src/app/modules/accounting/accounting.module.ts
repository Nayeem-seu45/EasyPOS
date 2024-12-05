import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountingRoutingComponents, AccountingRoutingModule } from './accounting-routing.module';
import { AppSharedModule } from 'src/app/shared/app-shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { API_BASE_URL } from '../generated-clients/api-service';

@NgModule({
  declarations: [
    ...AccountingRoutingComponents
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,
    AccountingRoutingModule,
  ],
  providers: [
    { provide: API_BASE_URL, useValue: environment.API_BASE_URL },
  ]
})
export class AccountingModule { }
