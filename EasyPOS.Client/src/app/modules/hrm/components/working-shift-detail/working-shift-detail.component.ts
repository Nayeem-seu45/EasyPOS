import { Component, inject, OnInit } from '@angular/core';
import { WorkingShiftModel, WorkingShiftsClient } from 'src/app/modules/generated-clients/api-service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { CommonConstants } from 'src/app/core/contants/common';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { WorkingShiftMapper } from './WorkingShiftMapper ';

@Component({
  selector: 'app-working-shift-detail',
  templateUrl: './working-shift-detail.component.html',
  styleUrl: './working-shift-detail.component.scss',
  providers: [WorkingShiftsClient]
})
export class WorkingShiftDetailComponent implements OnInit {
  id: string;
  item: WorkingShiftModel;
  optionsDataSources: any;


  protected toast: ToastService = inject(ToastService);
  protected customDialogService: CustomDialogService = inject(CustomDialogService)

  constructor(private entityClient: WorkingShiftsClient) { }

  ngOnInit() {
    this.id = this.customDialogService.getConfigData();
    this.getById(this.id);
  }

  protected cancel() {
    this.customDialogService.close(false);
  }

  protected onFormSubmit() {
    if (!this.id || this.id === CommonConstants.EmptyGuid) {
      this.save();
    } else {
      this.update();
    }
  }

  protected save() {
    const createCommand = WorkingShiftMapper.toCreateCommand(this.item);
    this.entityClient.create(createCommand).subscribe({
      next: () => {
        this.toast.created();
        this.customDialogService.close(true);
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  protected update() {
    const updateCommand = WorkingShiftMapper.toUpdateCommand(this.item);
    this.entityClient.update(updateCommand ).subscribe({
      next: () => {
        this.toast.updated();
        this.customDialogService.close(true);
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  protected getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: WorkingShiftModel) => {
        this.item = res;
        this.optionsDataSources = res.optionsDataSources;
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }
}


