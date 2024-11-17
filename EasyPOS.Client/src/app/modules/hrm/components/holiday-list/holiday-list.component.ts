import { Component, inject } from '@angular/core';
import { HolidayDetailComponent } from '../holiday-detail/holiday-detail.component';
import { HolidaysClient } from 'src/app/modules/generated-clients/api-service';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-holiday-list',
  templateUrl: './holiday-list.component.html',
  styleUrl: './holiday-list.component.scss',
  providers: [HolidaysClient]
})
export class HolidayListComponent {
  detailComponent = HolidayDetailComponent;
  pageId = '5a034fd0-e5f4-4ada-6e42-08dd0659aaf7'

  config?: Partial<DynamicDialogConfig> = {width: '600px'}

  entityClient: HolidaysClient = inject(HolidaysClient);
}
