import { Directive, Inject, inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { CustomDialogService } from "../../services/custom-dialog.service";
import { ToastService } from "../../services/toast.service";
import { ENTITY_CLIENT } from "../../injection-tokens/tokens";
import { CommonUtils } from "../../Utilities/common-utilities";

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
  mapCreateResponse: boolean = false;
  public optionsDataSources = {};

  protected get f() {
    return this.form.controls;
  }

  // Dependency injections
  protected toast: ToastService = inject(ToastService);
  protected customDialogService: CustomDialogService = inject(CustomDialogService)
  protected fb: FormBuilder = inject(FormBuilder);

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
  protected onSubmit(actionData?: any) {

    if (this.form.invalid) {
      this.toast.showError('Form is invalid.');
      return;
    }

     // Prepare the command object with optional action-based customization
     let command = { ...this.form.value };
     command = this.beforeActionProcess(command, actionData);

    if (!this.id || this.id === this.emptyGuid) {
      this.save(command);
    } else {
      this.update(command);
    }
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

}