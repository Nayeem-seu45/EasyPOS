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
import { DragOverlayDirective } from 'src/app/shared/direvtives/drag-overlay.directive';
import { ConditionalDraggableDirective } from 'src/app/shared/direvtives/conditional-draggable.directive';



@NgModule({
  declarations: [
    ...DashboardRoutingComponents,
    DragOverlayDirective,
    ConditionalDraggableDirective
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
    DragDropModule 
  ]
})
export class DashboardModule { }
