import { Component, inject } from '@angular/core';
import { CountStockDetailComponent } from '../count-stock-detail/count-stock-detail.component';
import { CountStocksClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-count-stock-list',
  templateUrl: './count-stock-list.component.html',
  styleUrl: './count-stock-list.component.scss',
  providers: [CountStocksClient]
})
export class CountStockListComponent {
  detailComponent = CountStockDetailComponent;
  pageId = 'e514d626-960c-439b-03ab-08dcfaab86f4';

  entityClient: CountStocksClient = inject(CountStocksClient);
}
