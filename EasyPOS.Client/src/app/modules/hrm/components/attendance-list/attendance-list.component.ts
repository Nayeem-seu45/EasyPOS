import { Component, inject } from '@angular/core';
import { AttendanceDetailComponent } from '../attendance-detail/attendance-detail.component';
import { AttendancesClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-attendance-list',
  templateUrl: './attendance-list.component.html',
  styleUrl: './attendance-list.component.scss',
  providers: [AttendancesClient]
})
export class AttendanceListComponent {
  detailComponent = AttendanceDetailComponent;
  pageId = 'ece1894e-2b0e-416a-6e43-08dd0659aaf7'

  entityClient: AttendancesClient = inject(AttendancesClient);
}
