import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DraggableAreaDirective } from './directives/draggable-area.directive';

@NgModule({
  declarations: [
    DraggableAreaDirective
  ],
  imports: [
    CommonModule,
  ],
  exports: [
    DraggableAreaDirective
  ]
})
export class DraggablePositionModule { }
