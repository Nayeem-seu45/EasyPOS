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
  selector: '[appHasAllPermissions]'
})
export class HasAllPermissionsDirective implements OnInit, OnDestroy {
  private permissionSubscription: Subscription | null = null;
  
  @Input() appHasAllPermissions!: string[];
  @Input() appHasAllPermissionsElse?: TemplateRef<any>;

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private permissionService: PermissionService
  ) {}

  ngOnInit() {
    this.checkPermission();
  }

  ngOnDestroy() {
    this.permissionSubscription?.unsubscribe();
  }

  private checkPermission() {
    const hasAllPermissions = this.permissionService.hasAllPermissions(this.appHasAllPermissions);
    
    this.viewContainer.clear();
    
    if (hasAllPermissions) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else if (this.appHasAllPermissionsElse) {
      this.viewContainer.createEmbeddedView(this.appHasAllPermissionsElse);
    }
  }
}