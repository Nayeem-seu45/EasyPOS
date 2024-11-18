import { Component, Directive, Inject, inject, OnInit } from "@angular/core";
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

  protected toast: ToastService = inject(ToastService);
  protected customDialogService: CustomDialogService = inject(CustomDialogService)
  protected fb: FormBuilder = inject(FormBuilder);

  constructor(@Inject(ENTITY_CLIENT) protected entityClient: any) { }

  ngOnInit() {
    this.id = this.customDialogService.getConfigData();
    this.initializeFormGroup();
    this.getById(this.id);
  }

  protected abstract initializeFormGroup(): void;

  protected cancel() {
    this.customDialogService.close(false);
  }

  protected onSubmit() {

    if (this.form.invalid) {
      this.toast.showError('Form is invalid.');
      return;
    }

    if (!this.id || this.id === this.emptyGuid) {
      this.save();
    } else {
      this.update();
    }
  }

  protected save() {
    let createCommand = { ...this.form.value };
    createCommand = this.beforeActionProcess(createCommand);
    this.entityClient.create(createCommand).subscribe({
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

  protected update() {
    let updateCommand = { ...this.form.value };
    updateCommand = this.beforeActionProcess(updateCommand);
    this.entityClient.update(updateCommand).subscribe({
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
    * Optional method to process the command object before API calls.
    * Child classes can override this method as needed.
    */
  protected beforeActionProcess(command: any): any {
    return command; // Default implementation does nothing
  }

  protected postActionProcess() {

  }

  protected getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: any) => {
        if (!this.mapCreateResponse && id && id !== this.emptyGuid) {
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
}