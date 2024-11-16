import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DepartmentDetailComponent } from './components/department-detail/department-detail.component';
import { DepartmentListComponent } from './components/department-list/department-list.component';
import { DesignationDetailComponent } from './components/designation-detail/designation-detail.component';
import { DesignationListComponent } from './components/designation-list/designation-list.component';
import { EmployeeDetailComponent } from './components/employee-detail/employee-detail.component';
import { EmployeeListComponent } from './components/employee-list/employee-list.component';
import { WorkingShiftDetailComponent } from './components/working-shift-detail/working-shift-detail.component';
import { WorkingShiftListComponent } from './components/working-shift-list/working-shift-list.component';
import { HolidayDetailComponent } from './components/holiday-detail/holiday-detail.component';
import { HolidayListComponent } from './components/holiday-list/holiday-list.component';
import { AttendanceDetailComponent } from './components/attendance-detail/attendance-detail.component';
import { AttendanceListComponent } from './components/attendance-list/attendance-list.component';
import { LeaveRequestListComponent } from './components/leave-request-list/leave-request-list.component';
import { LeaveRequestDetailComponent } from './components/leave-request-detail/leave-request-detail.component';
import { LeaveTypeListComponent } from './components/leave-type-list/leave-type-list.component';
import { LeaveTypeDetailComponent } from './components/leave-type-detail/leave-type-detail.component';

const routes: Routes = [
  {path: 'departments', component: DepartmentListComponent},
  {path: 'designations', component: DesignationListComponent},
  {path: 'employees', component: EmployeeListComponent},
  {path: 'working-shifts', component: WorkingShiftListComponent},
  {path: 'holidays', component: HolidayListComponent},
  {path: 'attendances', component: AttendanceListComponent},
  {path: 'leaves', component: LeaveRequestListComponent},
  {path: 'leave-types', component: LeaveTypeListComponent},
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class HRMRoutingModule { }

export const HRMRoutingComponents = [
  DepartmentListComponent,
  DepartmentDetailComponent,
  DesignationListComponent,
  DesignationDetailComponent,
  EmployeeListComponent,
  EmployeeDetailComponent,
  WorkingShiftListComponent,
  WorkingShiftDetailComponent,
  HolidayListComponent,
  HolidayDetailComponent,
  AttendanceListComponent,
  AttendanceDetailComponent,
  LeaveRequestListComponent,
  LeaveRequestDetailComponent,
  LeaveTypeListComponent,
  LeaveTypeDetailComponent
]
