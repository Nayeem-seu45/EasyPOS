import { Directive, ElementRef, HostListener, Input, Renderer2, Output, EventEmitter } from '@angular/core';

@Directive({
  selector: '[appDraggableArea]'
})
export class DraggableAreaDirective {
  @Input() draggableArea = 40; // Draggable area height
  private offsetTop = 10; // Offset from the top
  @Output() dragStart = new EventEmitter<DragEvent>(); // Event emitter for drag start

  constructor(private el: ElementRef, private renderer: Renderer2) {}

  @HostListener('mouseenter', ['$event'])
  @HostListener('mousemove', ['$event'])
  onMouseMove(event: MouseEvent) {
    if (this.isInDraggableArea(event)) {
      this.renderer.setStyle(this.el.nativeElement, 'cursor', 'move');
    } else {
      this.renderer.setStyle(this.el.nativeElement, 'cursor', 'default');
    }
  }

  @HostListener('dragstart', ['$event'])
  onDragStart(event: DragEvent) {
    const rect = this.el.nativeElement.getBoundingClientRect();
    const draggableAreaStart = rect.top + this.offsetTop; // 10px offset
    const draggableAreaEnd = draggableAreaStart + this.draggableArea; // 40px area

    if (event.clientY >= draggableAreaStart && event.clientY <= draggableAreaEnd) {
      this.dragStart.emit(event); // Emit drag start event if within draggable area
    } else {
      event.preventDefault(); // Prevent the drag if outside the draggable area
    }
  }

  private isInDraggableArea(event: MouseEvent): boolean {
    const rect = this.el.nativeElement.getBoundingClientRect();
    return event.clientY >= rect.top + this.offsetTop && event.clientY <= rect.top + this.offsetTop + this.draggableArea;
  }
}
