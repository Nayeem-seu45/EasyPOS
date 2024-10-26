import { Directive, ElementRef, HostListener, Input, Renderer2 } from '@angular/core';

@Directive({
  selector: '[conditionalDraggable]'
})
export class ConditionalDraggableDirective {
  @Input() draggableArea = 40; // Draggable area of 40px
  private offsetTop = 10; // 10px offset from the top

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
    return event.clientY >= rect.top + this.offsetTop && event.clientY <= rect.top + this.offsetTop + this.draggableArea;
  }
}
