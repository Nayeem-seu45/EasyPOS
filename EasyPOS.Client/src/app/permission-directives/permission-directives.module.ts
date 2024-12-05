import { NgModule } from '@angular/core';
import { DisableIfNoPermissionDirective } from './disable-if-no-permission.directive';
import { HasAllPermissionsDirective } from './has-all-permission.directive';
import { HasAnyPermissionDirective } from './has-any-permission.directive';
import { HasPermissionDirective } from './has-permission.directive';

// Module to declare and export directives
@NgModule({
  declarations: [
    HasPermissionDirective,
    HasAnyPermissionDirective,
    HasAllPermissionsDirective,
    DisableIfNoPermissionDirective
  ],
  exports: [
    HasPermissionDirective,
    HasAnyPermissionDirective,
    HasAllPermissionsDirective,
    DisableIfNoPermissionDirective
  ]
})
export class PermissionDirectivesModule { }
