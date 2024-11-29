import { Component, Inject, ViewChild } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { GetDateRangeTotalCountQuery, LeaveRequestModel, LeaveRequestsClient, LeaveStatus } from 'src/app/modules/generated-clients/api-service';
import { DatePipe } from '@angular/common';
import { DateUtilService } from 'src/app/shared/Utilities/date-util.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { CommonConstants } from 'src/app/core/contants/common';
import { InputDatepickerComponent } from 'src/app/shared/components/input-datepicker/input-datepicker.component';

@Component({
  selector: 'app-leave-request-detail',
  templateUrl: './leave-request-detail.component.html',
  styleUrl: './leave-request-detail.component.scss',
  providers: [{ provide: ENTITY_CLIENT, useClass: LeaveRequestsClient }, DatePipe]
})
export class LeaveRequestDetailComponent extends BaseDetailComponent {

  minEndDate: Date | null = null;
  LeaveStatus = LeaveStatus;
  isSubmitted: boolean = false;
  hasApprovalPermission: boolean = false;

  @ViewChild('startDatePicker') startDatePicker: InputDatepickerComponent;
  @ViewChild('endDatePicker') endDatePicker: InputDatepickerComponent;

  constructor(@Inject(ENTITY_CLIENT) entityClient: LeaveRequestsClient,
    private dateUtil: DateUtilService,
    private datePipe: DatePipe) {
    super(entityClient)
  }

  override onActionHandler(formData: any, actionData: any) {
    console.log(formData);
    console.log(actionData);
    if (actionData === LeaveStatus.Submitted) {
      this.submit(formData)
    } else if (actionData === LeaveStatus.Approved || actionData === LeaveStatus.Rejected){
      this.approval(formData, actionData)
    }
  }

  

  onStartDateChange(date: any): void {
    if (!this.checkEmployeeAndLeaveType()) {
      this.emptyStartAndEndDate();
      return;
    }

    // Set minEndDate based on selected startDate
    this.minEndDate = new Date(this.dateUtil.convertToDateOnlyFormat(date));

    // Validate endDate if it exists
    this.validateEndDate();
  }


  onEndDateChange(date: any): void {
    if (!this.checkEmployeeAndLeaveType()) {
      this.f['endDate']?.setValue(null);
      return;
    }

    // Validate startDate and endDate
    this.validateStartDate();
  }

  private validateEndDate(): void {
    const endDate = this.f['endDate']?.value;
    if (endDate && this.dateUtil.isValidDateStringFormat(endDate)) {
      this.getDateRangeCount();
    }
  }

  private validateStartDate(): void {
    const startDate = this.f['startDate']?.value;
    if (startDate && this.dateUtil.isValidDateStringFormat(startDate)) {
      this.getDateRangeCount();
    }
  }

  private emptyStartAndEndDate() {
    this.f['startDate']?.setValue(null, { emitEvent: false });
    this.f['endDate']?.setValue(null, { emitEvent: false });

    // Clear date pickers without triggering events
    if (this.startDatePicker) {
      this.startDatePicker.writeValue(null);
    }
    if (this.endDatePicker) {
      this.endDatePicker.writeValue(null);
    }
  }

  private checkEmployeeAndLeaveType(): boolean {
    const employeeId = this.f['employeeId']?.value;
    const leaveTypeId = this.f['leaveTypeId']?.value;

    if (!employeeId || employeeId === CommonConstants.EmptyGuid) {
      this.toast.showWarn('Select Employee');
      return false;
    }

    if (!leaveTypeId || leaveTypeId === CommonConstants.EmptyGuid) {
      this.toast.showWarn('Select Leave Type');
      return false;
    }

    return true;
  }


  private submit(command: any) {
    command = {
      ...command,
      startDate: this.dateUtil.convertToDateOnlyFormat(command.startDate),
      endDate: this.dateUtil.convertToDateOnlyFormat(command.endDate),
      isSubmitted: true
    };
    this.entityClient.update(command).subscribe({
      next: () => {
        this.toast.showSuccess('Submitted Successfully', 'Submitted');
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

  private approval(command: any, actionData: any) {
    command = {
      ...command,
      startDate: this.dateUtil.convertToDateOnlyFormat(command.startDate),
      endDate: this.dateUtil.convertToDateOnlyFormat(command.endDate),
      approvalAction: actionData
    };
    this.entityClient.approval(command).subscribe({
      next: () => {
          if (actionData === LeaveStatus.Approved) {
            this.toast.showSuccess('Approved Successfully', 'Approved');
          } else if (actionData === LeaveStatus.Rejected){
            this.toast.showSuccess('Rejected Successfully', 'Rejected');
          }
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

  private isRequestSubmitted() {
    this.isSubmitted = this.item?.leaveStatus >= LeaveStatus.Submitted;

    if (this.isSubmitted)
      this.diableField('leaveTypeId');

  }

  formatDate(date: any): string {
    if (!date) return '';
    const dateObj = new Date(date);
    return this.datePipe.transform(dateObj, 'dd/MM/yyyy')!;
  }

  override getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: LeaveRequestModel) => {
        if (id && id !== this.emptyGuid) {
          this.item = res;
          this.item.startDate = this.formatDate(res.startDate),
            this.item.endDate = this.formatDate(res.endDate)

        } else {
          this.item = new LeaveRequestModel();
          this.item.employeeId = res.employeeId
        }
        this.optionsDataSources = res.optionsDataSources;
        this.isRequestSubmitted();
        this.form.patchValue({
          ...this.item
        });
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  override beforeActionProcess(command: any, actionData: any): any {
    return {
      ...command,
      startDate: this.dateUtil.convertToDateOnlyFormat(command.startDate),
      endDate: this.dateUtil.convertToDateOnlyFormat(command.endDate),
    };
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      employeeId: [null],
      leaveTypeId: [null],
      startDate: [null],
      endDate: [null],
      totalDays: [{ value: null, disabled: true }],
      statusId: [null],
      attachmentUrl: [null],
      reason: [null]
    });
  }

  override applyFieldPermissions(): void {
    // Enable or disable the employeeId field based on the user's permission
    if (this.permissionService.hasPermission('Permissions.LeaveRequests.ModifyEmployee')) {
      this.form.get('employeeId')?.enable();
    } else {
      this.form.get('employeeId')?.disable();
    }
    if (this.permissionService.hasPermission('Permissions.LeaveRequests.Approval')) {
      this.hasApprovalPermission = true;
    }
  }

  private getDateRangeCount() {
    const query = new GetDateRangeTotalCountQuery();
    query.employeeId = this.f['employeeId']?.value;
    query.leaveTypeId = this.f['leaveTypeId']?.value;
    const startDate = this.f['startDate']?.value;
    const endDate = this.f['endDate']?.value;

    if (!query?.employeeId || query?.employeeId == CommonConstants.EmptyGuid) {
      this.toast.showWarn('Select Employee');
      return;
    } else if (!query?.leaveTypeId || query?.leaveTypeId == CommonConstants.EmptyGuid) {
      this.toast.showWarn('Select Leave Type');
      return;
    } else if (!startDate || !startDate) {
      return;
    }
    query.startDate = new Date(this.dateUtil.convertToDateOnlyFormat(startDate)),
      query.endDate = new Date(this.dateUtil.convertToDateOnlyFormat(endDate)),

      this.entityClient.getDateRangeTotalCount(query).subscribe({
        next: (totalDays: number) => {
          this.f['totalDays'].setValue(totalDays);
        }, error: (error) => {
          this.toast.showWarn(CommonUtils.getErrorMessage(error[0]))
        }
      });
  }

}
