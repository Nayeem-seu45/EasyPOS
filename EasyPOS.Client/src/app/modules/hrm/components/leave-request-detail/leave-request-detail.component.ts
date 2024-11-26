import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { GetDateRangeTotalCountQuery, LeaveRequestsClient } from 'src/app/modules/generated-clients/api-service';
import { DatePipe } from '@angular/common';
import { DateUtilService } from 'src/app/shared/Utilities/date-util.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { CommonConstants } from 'src/app/core/contants/common';

@Component({
  selector: 'app-leave-request-detail',
  templateUrl: './leave-request-detail.component.html',
  styleUrl: './leave-request-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: LeaveRequestsClient}, DatePipe]
})
export class LeaveRequestDetailComponent extends BaseDetailComponent {

  minEndDate: Date | null = null;
  maxStartDate: Date | null = null;

  constructor(@Inject(ENTITY_CLIENT) entityClient: LeaveRequestsClient,
  private datePipe: DatePipe,
  private dateUtil: DateUtilService){
    super(entityClient)
  }

  override ngOnInit(): void {
    super.ngOnInit();

    // Subscribe to startDate changes
    // this.form.get('startDate')?.valueChanges.subscribe((startDateValue: any) => {
    //   if (startDateValue) {
    //     this.minEndDate = new Date(this.dateUtil.convert_dmy_to_ymd(startDateValue)); 
    //     if(this.f['endDate']?.value){
    //       this.getDateRangeCount();
    //       this.f['startDate']?.setValue(null);
    //     }
    //   } else {
    //     this.minEndDate = null;
    //   }
    // });

    // this.form.get('endDate')?.valueChanges.subscribe((endDateValue: any) => {
    //   if (endDateValue) {
    //     const convertedDate = this.dateUtil.convert_dmy_to_ymd(endDateValue);
    //     this.maxStartDate = new Date(convertedDate); 
    //     if(this.f['startDate']?.value){
    //       this.getDateRangeCount();
    //       this.f['endDate']?.setValue(null, {});
    //     }
    //   } else {
    //     this.maxStartDate = null; 
    //   }
    // });
  }

  override onSubmit(){
    console.log(this.form.value)
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
    query.startDate = new Date(this.dateUtil.convert_dmy_to_ymd(startDate, '-')),
    query.endDate = new Date(this.dateUtil.convert_dmy_to_ymd(endDate, '-')),
    console.log(query)

    this.entityClient.getDateRangeTotalCount(query).subscribe({
      next: (totalDays: number) => {
        this.f['totalDays'].setValue(totalDays);
      }, error: (error) => {
        this.toast.showWarn(CommonUtils.getErrorMessage(error[0]))
      }
    });

    

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

  onEndDateChange(date){
    const employeeId = this.f['employeeId']?.value;
    const leaveTypeId = this.f['leaveTypeId']?.value;
    if(!employeeId || employeeId == CommonConstants.EmptyGuid){
      this.toast.showWarn('Select Employee');
      this.form.get('endDate')?.setValue(null, { emitEvent: false });
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

  override beforeActionProcess(command: any): any {
    return {
      ...command,
      // startDate: this.datePipe.transform(command.startDate, 'yyyy-MM-dd'),
      // endDate: this.datePipe.transform(command.endDate, 'yyyy-MM-dd'),
      startDate: this.dateUtil.convert_dmy_to_ymd(command.startDate, '-'),
      endDate: this.dateUtil.convert_dmy_to_ymd(command.endDate, '-'),
    };
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      employeeId: [null],
      leaveTypeId: [null],
      startDate: [null],
      endDate: [null],
      totalDays: [null],
      statusId: [null],
      attachmentUrl: [null],
      reason: [null]
    });
  }

}
