import { Component, inject } from '@angular/core';
import { CourierDetailComponent } from '../courier-detail/courier-detail.component';
import { CouriersClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-courier-list',
  templateUrl: './courier-list.component.html',
  styleUrl: './courier-list.component.scss',
  providers: [CouriersClient]
})
export class CourierListComponent {
  detailComponent = CourierDetailComponent;
  pageId = '0e002495-e4e4-434c-6683-08dcf6a0c463'

  entityClient: CouriersClient = inject(CouriersClient);
}
