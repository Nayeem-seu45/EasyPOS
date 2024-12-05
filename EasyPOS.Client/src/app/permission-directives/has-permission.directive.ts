import { 
  Directive, 
  Input, 
  TemplateRef, 
  ViewContainerRef, 
  OnInit, 
  OnDestroy
} from '@angular/core';
import { Subscription } from 'rxjs';
import { PermissionService } from '../core/auth/services/permission.service';

@Directive({
  selector: '[appHasPermission]'
})
export class HasPermissionDirective implements OnInit, OnDestroy {
  private permissionSubscription: Subscription | null = null;
  
  @Input() appHasPermission!: string;
  @Input() appHasPermissionElse?: TemplateRef<any>;

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private permissionService: PermissionService
  ) {}

  ngOnInit() {
    // Subscribe to permission changes
    this.permissionSubscription = this.permissionService.permissions$.subscribe(() => {
      this.checkPermission();
    });
  }

  ngOnDestroy() {
    this.permissionSubscription?.unsubscribe();
  }

  private checkPermission() {
    const hasPermission = this.permissionService.hasPermission(this.appHasPermission);
    
    this.viewContainer.clear();
    
    if (hasPermission) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else if (this.appHasPermissionElse) {
      this.viewContainer.createEmbeddedView(this.appHasPermissionElse);
    }
  }
}