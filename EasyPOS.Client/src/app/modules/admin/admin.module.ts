import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { AdminRoutingModule, adminRoutingComponents } from './admin-routing.module';
import { AppSharedModule } from 'src/app/shared/app-shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { API_BASE_URL } from '../generated-clients/api-service';
import { environment } from 'src/environments/environment';
import { DragDropModule } from 'primeng/dragdrop';
import { TabMenuModule } from 'primeng/tabmenu';
import { InputTextModule } from 'primeng/inputtext';
import { FileUploadModule } from 'primeng/fileupload';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TreeModule } from 'primeng/tree';
import { DraggablePositionModule } from 'src/app/others/draggable-position/draggable-position.module';
import { PermissionDirectivesModule } from 'src/app/permission-directives/permission-directives.module';

@NgModule({
  declarations: [
    ...adminRoutingComponents,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppSharedModule,
    AdminRoutingModule,
    DropdownModule,
    DragDropModule,
    TabMenuModule,
    InputTextModule,
    FileUploadModule,
    SelectButtonModule,
    TreeModule,
    DraggablePositionModule,
    PermissionDirectivesModule
  ],
  providers: [
    { provide: API_BASE_URL, useValue: environment.API_BASE_URL },
  ]
})
export class AdminModule { }
