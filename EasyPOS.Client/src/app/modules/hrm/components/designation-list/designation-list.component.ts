import { Component, inject } from '@angular/core';
import { DesignationDetailComponent } from '../designation-detail/designation-detail.component';
import { DesignationsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-designation-list',
  templateUrl: './designation-list.component.html',
  styleUrl: './designation-list.component.scss',
  providers: [DesignationsClient]
})
export class DesignationListComponent {
  detailComponent = DesignationDetailComponent;
  pageId = '730bb9f0-0bd2-4f17-6e3f-08dd0659aaf7'

  entityClient: DesignationsClient = inject(DesignationsClient);
}
