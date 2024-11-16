import { Component, inject } from '@angular/core';
import { WorkingShiftDetailComponent } from '../working-shift-detail/working-shift-detail.component';
import { WorkingShiftsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-working-shift-list',
  templateUrl: './working-shift-list.component.html',
  styleUrl: './working-shift-list.component.scss',
  providers: [WorkingShiftsClient]
})
export class WorkingShiftListComponent {
  detailComponent = WorkingShiftDetailComponent;
  pageId = 'fc1b7f6b-63ba-4535-6e41-08dd0659aaf7'

  entityClient: WorkingShiftsClient = inject(WorkingShiftsClient);
}
