import { 
  Directive, 
  Input, 
  TemplateRef, 
  ViewContainerRef, 
  OnInit, 
  OnDestroy
} from '@angular/core';
import { PermissionService } from '../core/auth/services/permission.service';
import { Subscription } from 'rxjs';

// Module to declare and export directives
@Directive({
  selector: '[appApplyPermissionIfHas]'
})
export class ApplyPermissionIfHasDirective implements OnInit, OnDestroy {
  @Input() appApplyPermissionIfHas!: string;
  @Input() appApplyPermissionIfHasElse?: TemplateRef<any>;
  private permissionSubscription: Subscription | null = null;

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
    const shouldApply = this.permissionService.applyPermissionIfHas(this.appApplyPermissionIfHas);
    
    this.viewContainer.clear();
    
    if (shouldApply) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else if (this.appApplyPermissionIfHasElse) {
      this.viewContainer.createEmbeddedView(this.appApplyPermissionIfHasElse);
    }
  }
}
