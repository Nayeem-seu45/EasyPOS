import { Component, ComponentFactoryResolver, ComponentRef, EventEmitter, Input, Output, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { SidebarService } from '../../services/sidebar.service';


@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {

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
  @Input() titleIcon: string = '';
  @Input() title: string = '';

  @Output() onShow = new EventEmitter<any>();
  @Output() onHide = new EventEmitter<any>();
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() sidebarClose = new EventEmitter<any>();

  @ViewChild('sidebarContent', { read: ViewContainerRef, static: true }) sidebarContent: ViewContainerRef;

  private componentRef: ComponentRef<any>;

  constructor(private sidebarService: SidebarService, private resolver: ComponentFactoryResolver) { }

  ngOnInit() {
    this.sidebarService.sidebarState$.subscribe(({ component, data, config }) => {
      if (config) {
        Object.keys(config).forEach(key => {
          if (this.hasOwnProperty(key)) {
            this[key] = config[key];
          }
        });
      }
      const factory = this.resolver.resolveComponentFactory(component);
      this.sidebarContent.clear();
      this.componentRef = this.sidebarContent.createComponent(factory);
      if (data) {
        Object.assign(this.componentRef.instance, data);
      }
      this.visible = true;
    });

    this.sidebarService.sidebarClose$.subscribe((returnData) => {
      this.visible = false;
      if (this.componentRef) {
        this.componentRef.destroy();
      }
      this.sidebarClose.emit(returnData);
    });
  }

  handleShow() {
    this.onShow.emit();
  }

  handleHide() {
    this.visible = false;
    this.onHide.emit();
    if (this.componentRef) {
      this.componentRef.destroy();
    }
  }

  handleVisibleChange(visible: boolean) {
    this.visibleChange.emit(visible);
  }

}
