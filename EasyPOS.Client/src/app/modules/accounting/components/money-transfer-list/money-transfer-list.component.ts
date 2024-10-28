import { Component, inject } from '@angular/core';
import { MoneyTransferDetailComponent } from '../money-transfer-detail/money-transfer-detail.component';
import { MoneyTransfersClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-money-transfer-list',
  templateUrl: './money-transfer-list.component.html',
  styleUrl: './money-transfer-list.component.scss',
  providers: [MoneyTransfersClient]
})
export class MoneyTransferListComponent {
  detailComponent = MoneyTransferDetailComponent;
  pageId = 'd7bfe126-dbee-4798-b3be-08dcf782b6da'

  entityClient: MoneyTransfersClient = inject(MoneyTransfersClient);
}
