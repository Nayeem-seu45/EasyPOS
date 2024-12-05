import { 
  Directive, 
  Input, 
  TemplateRef, 
  ViewContainerRef, 
  OnInit, 
  OnDestroy,
} from '@angular/core';
import { PermissionService } from '../core/auth/services/permission.service';
import { Subscription } from 'rxjs';

@Directive({
  selector: '[appHasAnyPermission]'
})
export class HasAnyPermissionDirective implements OnInit, OnDestroy {
  private permissionSubscription: Subscription | null = null;
  
  @Input() appHasAnyPermission!: string[];
  @Input() appHasAnyPermissionElse?: TemplateRef<any>;

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
    const hasAnyPermission = this.permissionService.hasAnyPermission(this.appHasAnyPermission);
    
    this.viewContainer.clear();
    
    if (hasAnyPermission) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else if (this.appHasAnyPermissionElse) {
      this.viewContainer.createEmbeddedView(this.appHasAnyPermissionElse);
    }
  }
}