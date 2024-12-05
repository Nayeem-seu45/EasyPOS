import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AccountsClient } from './auth-client.service';

@Injectable({
  providedIn: 'root',
})
export class PermissionService {
  private permissionsSubject = new BehaviorSubject<Set<string>>(new Set());
  public permissions$ = this.permissionsSubject.asObservable();

  constructor(private accountsClient: AccountsClient) { }

  /**
   * Load user permissions and update the permissions state
   * @param allowCache Whether to allow cached permissions (default: true)
   */
  loadPermissions(allowCache: boolean = true): void {
    this.accountsClient.getUserPermissions(allowCache).subscribe({
      next: (permits: string[]) => {
        // Normalize permissions by trimming and converting to lowercase
        const normalizedPermissions = new Set(
          permits.map((p) => p.trim().toLowerCase())
        );
        // Update the BehaviorSubject
        this.permissionsSubject.next(normalizedPermissions);
      },
      error: (error) => {
        console.error('Failed to load permissions:', error);
      },
    });
  }

  /**
   * Check if a specific permission exists otherwise always return true
   * @param permission Permission to check
   * @returns Boolean indicating permission existence
   */
  applyPermissionIfHas(permission: string | null = null): boolean {
    // If no permission is specified, return true (default access)
    if (!permission) return true;

    // Normalize the permission check
    const normalizedPermission = permission.trim().toLowerCase();

    return this.permissionsSubject.value.has(normalizedPermission);
  }


  /**
   * Check if a specific permission exists
   * @param permission Permission to check
   * @returns Boolean indicating permission existence
   */
  hasPermission(permission?: string): boolean {
    if (!permission) return false;
    return this.permissionsSubject.value.has(permission.trim().toLowerCase());
  }

  /**
   * Check if any of the specified permissions exist
   * @param permissions Array of permissions to check
   * @returns Boolean indicating if any permission exists
   */
  hasAnyPermission(permissions: string[]): boolean {
    if (!permissions || permissions.length === 0) return false;
    return permissions.some((permission) =>
      this.hasPermission(permission)
    );
  }

  /**
   * Check if all specified permissions exist
   * @param permissions Array of permissions to check
   * @returns Boolean indicating if all permissions exist
   */
  hasAllPermissions(permissions: string[]): boolean {
    if (!permissions || permissions.length === 0) return false;
    return permissions.every((permission) =>
      this.hasPermission(permission)
    );
  }

  /**
   * Clear all permissions
   */
  clearPermissions(): void {
    this.permissionsSubject.next(new Set());
  }

  /**
   * Get current permissions as an array
   * @returns Array of current permissions
   */
  getCurrentPermissions(): string[] {
    return Array.from(this.permissionsSubject.value);
  }


  // observable verions of loadPermissions
  /**
     * Load user permissions
     * @param allowCache Whether to allow cached permissions (default: true)
     * @returns Observable of permission loading process
     */
  // loadPermissions(allowCache: boolean = true): Observable<string[]> {
  //   return new Observable(observer => {
  //     this.accountsClient.getUserPermissions(allowCache).subscribe({
  //       next: (permits: string[]) => {
  //         // Normalize permissions by trimming and converting to lowercase
  //         const normalizedPermissions = new Set(
  //           permits.map(p => p.trim().toLowerCase())
  //         );

  //         // Update the BehaviorSubject
  //         this.permissionsSubject.next(normalizedPermissions);

  //         observer.next(permits);
  //         observer.complete();
  //       },
  //       error: (error) => {
  //         console.error('Failed to load permissions:', error);
  //         observer.error(error);
  //       }
  //     });
  //   });
  // }

}
