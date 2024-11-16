import { Component, inject } from '@angular/core';
import { EmployeeDetailComponent } from '../employee-detail/employee-detail.component';
import { EmployeesClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.scss',
  providers: [EmployeesClient]
})
export class EmployeeListComponent {
  detailComponent = EmployeeDetailComponent;
  pageId = 'b6ed8006-420d-43e0-6e40-08dd0659aaf7'

  entityClient: EmployeesClient = inject(EmployeesClient);
}
