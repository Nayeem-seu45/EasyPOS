import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommonSetupRoutingComponents, CommonSetupRoutingModule } from './common-setup-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { environment } from 'src/environments/environment';
import { API_BASE_URL, LookupsClient, SelectListsClient } from '../generated-clients/api-service';
import { AppSharedModule } from 'src/app/shared/app-shared.module';



@NgModule({
  declarations: [
    ...CommonSetupRoutingComponents
  ],
  imports: [
    CommonModule,
    CommonSetupRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,


  // PrimeNg Modules //
    // TableModule,
    // ButtonModule,
    // RippleModule,
    // MultiSelectModule,
    // InputTextModule,
    // InputTextareaModule,
    // DropdownModule,
    // RadioButtonModule,
    // InputNumberModule,
    // InputSwitchModule,
    // CalendarModule,
    // TooltipModule,
    // FileUploadModule,
    // ToolbarModule,
    // RatingModule,
    // TagModule,
    // DialogModule,
    // DynamicDialogModule,

  ],
  providers: [
    { provide: API_BASE_URL, useValue: environment.API_BASE_URL },
    LookupsClient,
    SelectListsClient,
  ]
})
export class CommonSetupModule { }
