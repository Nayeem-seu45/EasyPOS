import { Component, inject } from '@angular/core';
import { DepartmentDetailComponent } from '../department-detail/department-detail.component';
import { DepartmentsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-department-list',
  templateUrl: './department-list.component.html',
  styleUrl: './department-list.component.scss',
  providers: [DepartmentsClient]
})
export class DepartmentListComponent {
  detailComponent = DepartmentDetailComponent;
  pageId = '9e7ca3ca-6906-49a3-6e3e-08dd0659aaf7'

  entityClient: DepartmentsClient = inject(DepartmentsClient);
}
