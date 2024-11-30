import { inject, Injectable } from '@angular/core';
import { AccountsClient } from './auth-client.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {

  private permissions: Set<string> = new Set();

  private accountsClient: AccountsClient = inject(AccountsClient);

  loadPermissions(allowCache: boolean = true){
    this.accountsClient.getUserPermissions(allowCache).subscribe({
      next: (permits: string[]) => {
        this.permissions = new Set<string>(permits);
      },
      error: (error) => {
        console.error('Failed to load permissions:', error);
      }
    });
  }

  applyPermissionIfHas(permission: string = null): boolean {
    if(!permission) return true;
    return this.permissions.has(permission.trim());
  }

  hasPermission(permission?: string): boolean {
    if(!permission) return true;
    return this.permissions.has(permission.trim());
  }

  hasAnyPermission(permissions: string[]): boolean 
  { 
    if (permissions.length === 0) return false; 
    return permissions.some(permission => this.permissions.has(permission.trim())); 
  } 
    
  hasAllPermissions(permissions: string[]): boolean 
  { 
    if (permissions.length === 0) return false; 
    return permissions.every(permission => this.permissions.has(permission.trim())); 
  }

}
