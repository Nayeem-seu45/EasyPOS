import { Directive, ElementRef, Renderer2, HostListener } from '@angular/core';

@Directive({
  selector: '[appDragOverlay]'
})
export class DragOverlayDirective {

  private overlay: HTMLElement;

  constructor(private el: ElementRef, private renderer: Renderer2) {
    this.createOverlay();
  }

  private createOverlay() {
    this.overlay = this.renderer.createElement('div');
    this.renderer.setStyle(this.overlay, 'position', 'absolute');
    this.renderer.setStyle(this.overlay, 'top', '0');
    this.renderer.setStyle(this.overlay, 'left', '0');
    this.renderer.setStyle(this.overlay, 'width', '100%');
    this.renderer.setStyle(this.overlay, 'height', '25%');
    this.renderer.setStyle(this.overlay, 'cursor', 'move');
    this.renderer.setStyle(this.overlay, 'z-index', '1'); // Ensure overlay is on top

    this.renderer.appendChild(this.el.nativeElement, this.overlay);
    this.renderer.setStyle(this.el.nativeElement, 'position', 'relative'); // Ensure host element is positioned relative for the absolute overlay
  }

  @HostListener('mousedown', ['$event'])
  onMouseDown(event: MouseEvent) {
    if (event.target === this.overlay) {
      this.el.nativeElement.dispatchEvent(new MouseEvent('mousedown', event));
    }
  }
}
