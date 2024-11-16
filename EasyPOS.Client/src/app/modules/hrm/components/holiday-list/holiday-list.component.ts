import { Component, inject } from '@angular/core';
import { HolidayDetailComponent } from '../holiday-detail/holiday-detail.component';
import { HolidaysClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-holiday-list',
  templateUrl: './holiday-list.component.html',
  styleUrl: './holiday-list.component.scss',
  providers: [HolidaysClient]
})
export class HolidayListComponent {
  detailComponent = HolidayDetailComponent;
  pageId = '5a034fd0-e5f4-4ada-6e42-08dd0659aaf7'

  entityClient: HolidaysClient = inject(HolidaysClient);
}
