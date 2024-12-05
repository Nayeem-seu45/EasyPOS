import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { ChartModule } from 'primeng/chart';
import { MenuModule } from 'primeng/menu';
import { PanelMenuModule } from 'primeng/panelmenu';
import { StyleClassModule } from 'primeng/styleclass';
import { TableModule } from 'primeng/table';
import { DashboardRouting, DashboardRoutingComponents } from './dashboard-routing.module';
import { DragDropModule } from 'primeng/dragdrop';
import { DraggablePositionModule } from 'src/app/others/draggable-position/draggable-position.module';
import { PermissionDirectivesModule } from 'src/app/permission-directives/permission-directives.module';


@NgModule({
  declarations: [
    ...DashboardRoutingComponents,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ChartModule,
    MenuModule,
    TableModule,
    StyleClassModule,
    PanelMenuModule,
    ButtonModule,
    DashboardRouting,
    DragDropModule,
    DraggablePositionModule,
    PermissionDirectivesModule
  ]
})
export class DashboardModule { }
