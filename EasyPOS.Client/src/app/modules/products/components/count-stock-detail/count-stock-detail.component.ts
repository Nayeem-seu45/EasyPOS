import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { CountStocksClient, CountStockType } from 'src/app/modules/generated-clients/api-service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-count-stock-detail',
  templateUrl: './count-stock-detail.component.html',
  styleUrl: './count-stock-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: CountStocksClient}]
})
export class CountStockDetailComponent extends BaseDetailComponent {
  CountStockType = CountStockType;
  countStockTypeSelectList: any;
  constructor(@Inject(ENTITY_CLIENT) entityClient: CountStocksClient){
    super(entityClient)
  }

  override ngOnInit() {
    this.id = this.customDialogService.getConfigData();
    this.countStockTypeSelectList = CommonUtils.enumToArray(CountStockType);
    this.initializeFormGroup();
    this.getById(this.id);
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      warehouseId: [null],
      type: [CountStockType.Full],
      categoryIds: [null],
      brandIds: [null]
    });
  }

}
