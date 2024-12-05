import { 
  Directive, 
  Input, 
  OnInit, 
  OnDestroy,
  ElementRef,
  Renderer2
} from '@angular/core';
import { Subscription } from 'rxjs';
import { PermissionService } from '../core/auth/services/permission.service';

@Directive({
  selector: '[appDisableIfNoPermission]'
})
export class DisableIfNoPermissionDirective implements OnInit, OnDestroy {
  private permissionSubscription: Subscription | null = null;
  
  @Input() appDisableIfNoPermission!: string;

  constructor(
    private el: ElementRef,
    private renderer: Renderer2,
    private permissionService: PermissionService
  ) {}

  ngOnInit() {
    this.checkPermission();
  }

  ngOnDestroy() {
    this.permissionSubscription?.unsubscribe();
  }

  private checkPermission() {
    const hasPermission = this.permissionService.hasPermission(this.appDisableIfNoPermission);
    
    if (!hasPermission) {
      this.renderer.setAttribute(this.el.nativeElement, 'disabled', 'true');
      this.renderer.addClass(this.el.nativeElement, 'disabled');
    }
  }
}