import { Directive, Inject, inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { CustomDialogService } from "../../services/custom-dialog.service";
import { ToastService } from "../../services/toast.service";
import { ENTITY_CLIENT } from "../../injection-tokens/tokens";
import { CommonUtils } from "../../Utilities/common-utilities";
import { PermissionService } from "src/app/core/auth/services/permission.service";

@Directive({
  selector: '[baseDetail]', // Selector is required but can be generic
  providers: [
    CustomDialogService,
    FormBuilder
  ]
})
export abstract class BaseDetailComponent implements OnInit {
  emptyGuid = '00000000-0000-0000-0000-000000000000';
  form: FormGroup;
  id: string = '';
  item: any;
  public optionsDataSources = {};

  protected get f() {
    return this.form.controls;
  }

  protected get isEdit(): boolean{
    return this.id && this.id !== this.emptyGuid;
  }

  // Dependency injections
  protected toast: ToastService = inject(ToastService);
  protected customDialogService: CustomDialogService = inject(CustomDialogService)
  protected fb: FormBuilder = inject(FormBuilder);
  protected permissionService = inject(PermissionService);
  constructor(@Inject(ENTITY_CLIENT) protected entityClient: any) { }

  ngOnInit() {
    this.onInitialize();
  }

  /**
   * Initialization logic separated from `ngOnInit`.
   * Can be overridden by child components for custom initialization.
   */
  protected onInitialize(): void {
    this.id = this.customDialogService.getConfigData();
    this.initializeFormGroup();
    this.getById(this.id);
    this.applyFieldPermissions(); // Hook for permission handling
    this.afterOnInit(); // Hook for afterOnInit handling
  }

  afterOnInit(){

  }

  /**
   * Initialize the form group structure. Must be implemented by derived classes.
   */
  protected abstract initializeFormGroup(): void;

  /**
   * Close the modal dialog.
   */
  protected cancel() {
    this.customDialogService.close(false);
  }

  /**
   * Submits the form data.
   */
  protected onFormSubmit(actionData?: any) {

    // Mark all fields as touched to trigger validation
    // Object.keys(this.form.controls).forEach(field => {
    //   const control = this.form.get(field);
    //   control?.markAsTouched();
    // });

    if (this.form.invalid) {
      this.toast.showError('Form is invalid.');
      return;
    }

     // Prepare the command object with optional action-based customization
     let formData = { ...this.form.getRawValue() };
     formData = this.beforeActionProcess(formData, actionData);

    if (!this.id || this.id === this.emptyGuid) {
      this.save(formData);
    } else {
      this.update(formData);
    }
  }

  protected onActionClick(event: Event,  actionData?: any): void {

     // Prevent default form submission
     event.preventDefault();
    
     // Stop event propagation to prevent triggering ngSubmit
     event.stopPropagation();

    // Mark all fields as touched to trigger validation
    Object.keys(this.form.controls).forEach(field => {
      const control = this.form.get(field);
      control?.markAsTouched();
    });

    if (this.form.invalid) {
      this.toast.showError('Form is invalid.');
      return;
    }

    this.onActionHandler(this.form.getRawValue(), actionData)
  }

  /**
   * Handles data fetching by ID.
   */
  protected getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: any) => {
        if (id && id !== this.emptyGuid) {
          this.item = res;
        }
        this.optionsDataSources = res.optionsDataSources;
        this.form.patchValue({
          ...this.item
        });
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  /**
  * Save operation. Executes the creation logic.
  */
  protected save(command: any) {
    // let command = { ...this.form.value };
    // command = this.beforeActionProcess(command) || command;
    this.entityClient.create(command).subscribe({
      next: () => {
        this.toast.created();
        this.customDialogService.close(true);
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      },
      complete: () => {
        this.postActionProcess();
      }
    });
  }

  /**
   * Update operation. Executes the update logic.
   */
  protected update(command: any) {
    // let command = { ...this.form.value };
    // command = this.beforeActionProcess(command);
    this.entityClient.update(command).subscribe({
      next: () => {
        this.toast.updated();
        this.customDialogService.close(true);
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      },
      complete: () => {
        this.postActionProcess();
      }
    });
  }

  protected diableField(fieldName: string){
    this.form.get(fieldName)?.disable();
  }

  protected enableField(fieldName: string){
    this.form.get(fieldName)?.enable();
  }

  /**
   * Hook to process data before save/update.
   * Can be overridden by child classes for custom logic.
   */
  protected beforeActionProcess(command?: any, data?: any): any {
    return command; // Default implementation does nothing
  }

  /**
   * Hook executed after save/update.
   * Can be overridden by child classes for custom logic.
   */
  protected postActionProcess(actionData?: any) {

  }

  /**
   * Can be overridden by child components to enable/disable fields based on permissions.
   */
  protected applyFieldPermissions(): void {
    // Default behavior: No field permissions applied.
  }

  protected updateFieldsBasedOnPermissions(permission: string, fieldNames: string | string[], enable: boolean): void {
    const fields = Array.isArray(fieldNames) ? fieldNames : [fieldNames];
  
    fields.forEach(fieldName => {
      const field = this.form.get(fieldName);
      if (field) {
        if (enable && this.permissionService.hasPermission(permission)) {
          field.enable();
        } else {
          field.disable();
        }
      }
    });
  }
  
  protected enableFieldsBasedOnPermissions(permission: string, fieldNames: string | string[]): void {
    this.updateFieldsBasedOnPermissions(permission, fieldNames, true);
  }
  
  protected disableFieldsBasedOnPermissions(permission: string, fieldNames: string | string[]): void {
    this.updateFieldsBasedOnPermissions(permission, fieldNames, false);
  }
  

   /**
   * Hook to handle on onActionClick button clicked.
   * Can be overridden by child classes for custom logic.
   */
   protected onActionHandler(formData?: any, actionData?: any) {

  }

}