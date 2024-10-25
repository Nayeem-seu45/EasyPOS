import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';


@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent{

  @Input() template: TemplateRef<any> = null;
  @Input() visible: boolean = null;
  @Input() appendTo: string = null;
  @Input() blockScroll: boolean = false;
  @Input() style: {} = null;
  @Input() styleClass: string = null;
  @Input() ariaCloseLabel: string = null;
  @Input() autoZIndex: boolean = true;
  @Input() baseZIndex: number = 0;
  @Input() modal: boolean = true;
  @Input() dismissible: boolean = true;
  @Input() showCloseIcon: boolean = true;
  @Input() closeOnEscape: boolean = true;
  @Input() transitionOptions: string = "150ms cubic-bezier(0, 0, 0.2, 1)";
  @Input() position: string = 'right';
  @Input() fullScreen: boolean = null;

  @Output() onShow = new EventEmitter<any>();
  @Output() onHide = new EventEmitter<any>();
  @Output() visibleChange = new EventEmitter<boolean>();


  handleShow() {
    this.onShow.emit();
  }

  handleHide() {
    this.onHide.emit();
  }

  handleVisibleChange(visible: boolean) {
    this.visibleChange.emit(visible);
  }

}
