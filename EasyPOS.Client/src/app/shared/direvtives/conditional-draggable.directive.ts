import { Directive, ElementRef, HostListener, Input, Renderer2 } from '@angular/core';

@Directive({
  selector: '[conditionalDraggable]'
})
export class ConditionalDraggableDirective {
  @Input() draggableArea = 0.25; // Top 25% by default

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

  private isInDraggableArea(event: MouseEvent): boolean {
    const rect = this.el.nativeElement.getBoundingClientRect();
    const draggableHeight = rect.height * this.draggableArea;
    return event.clientY <= rect.top + draggableHeight;
  }
}
