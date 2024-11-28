import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { GetDateRangeTotalCountQuery, LeaveRequestModel, LeaveRequestsClient, LeaveStatus } from 'src/app/modules/generated-clients/api-service';
import { DatePipe } from '@angular/common';
import { DateUtilService } from 'src/app/shared/Utilities/date-util.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { CommonConstants } from 'src/app/core/contants/common';
import { PermissionService } from 'src/app/core/auth/services/permission.service';

@Component({
  selector: 'app-leave-request-detail',
  templateUrl: './leave-request-detail.component.html',
  styleUrl: './leave-request-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: LeaveRequestsClient}, DatePipe]
})
export class LeaveRequestDetailComponent extends BaseDetailComponent {

  minEndDate: Date | null = null;
  maxStartDate: Date | null = null;
  LeaveStatus = LeaveStatus;

  constructor(@Inject(ENTITY_CLIENT) entityClient: LeaveRequestsClient,
  private dateUtil: DateUtilService,
  private permissionService: PermissionService){
    super(entityClient)
  }

  override onSubmit(actionData?: any){
    if (!actionData) {
      const submitButton = document.activeElement as HTMLButtonElement;
      actionData = submitButton?.value ? parseInt(submitButton.value, 10) : undefined;
    }
  
    console.log(this.form.value);
    console.log(actionData);
  }

  onStartDateChange(date){
    const employeeId = this.f['employeeId']?.value;
    const leaveTypeId = this.f['leaveTypeId']?.value;
    if(!employeeId || employeeId == CommonConstants.EmptyGuid){
      this.toast.showWarn('Select Employee');
      this.form.patchValue({'endDate': null}, { emitEvent: false });
      return;
    } else if(!leaveTypeId || leaveTypeId == CommonConstants.EmptyGuid) {
      this.toast.showWarn('Select Leave Type');
      this.form.patchValue({'endDate': null}, { emitEvent: false });
      return;
    }
    this.minEndDate = new Date(this.dateUtil.convert_dmy_to_ymd(date)); 
    if(this.f['endDate']?.value){
      this.getDateRangeCount();
      // this.form.patchValue({'startDate': null}, { emitEvent: false });
    }
  }

  onEndDateChange(date: any){
    const employeeId = this.f['employeeId']?.value;
    const leaveTypeId = this.f['leaveTypeId']?.value;
    if(!employeeId || employeeId == CommonConstants.EmptyGuid){
      this.toast.showWarn('Select Employee');
      this.form.get('endDate')?.setValue(null);
      return;
    } else if(!leaveTypeId || leaveTypeId == CommonConstants.EmptyGuid) {
      this.toast.showWarn('Select Leave Type');
      this.form.get('endDate')?.setValue(null, { emitEvent: false });
      return;
    }
    this.maxStartDate = new Date(this.dateUtil.convert_dmy_to_ymd(date)); 
    if(this.f['startDate']?.value){
      this.getDateRangeCount();
      // this.form.patchValue({'endDate': null}, { emitEvent: false });
    }

  }

  override getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: LeaveRequestModel) => {
        if (id && id !== this.emptyGuid) {
          this.item = res;
        } else {
          this.item = new LeaveRequestModel();
          this.item.employeeId = res.employeeId
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

  override beforeActionProcess(command: any, actionData: any): any {
    const isSubmitted = actionData === 'submitted';
    return {
      ...command,
      startDate: this.dateUtil.convert_dmy_to_ymd(command.startDate),
      endDate: this.dateUtil.convert_dmy_to_ymd(command.endDate),
      isSubmitted: isSubmitted
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
  }

  private getDateRangeCount(){
    const query = new GetDateRangeTotalCountQuery();
    query.employeeId = this.f['employeeId']?.value;
    query.leaveTypeId = this.f['leaveTypeId']?.value;
    const startDate = this.f['startDate']?.value;
    const endDate = this.f['endDate']?.value;

    if(!query?.employeeId || query?.employeeId == CommonConstants.EmptyGuid){
      this.toast.showWarn('Select Employee');
      return;
    } else if(!query?.leaveTypeId || query?.leaveTypeId == CommonConstants.EmptyGuid) {
      this.toast.showWarn('Select Leave Type');
      return;
    } else if(!startDate || !startDate){
      return;
    }
    query.startDate = new Date(this.dateUtil.convert_dmy_to_ymd(startDate)),
    query.endDate = new Date(this.dateUtil.convert_dmy_to_ymd(endDate)),

    this.entityClient.getDateRangeTotalCount(query).subscribe({
      next: (totalDays: number) => {
        this.f['totalDays'].setValue(totalDays);
      }, error: (error) => {
        this.toast.showWarn(CommonUtils.getErrorMessage(error[0]))
      }
    });
  }

}
