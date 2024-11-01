import { Component, inject } from '@angular/core';
import { ProductAdjustmentDetailComponent } from '../product-adjustment-detail/product-adjustment-detail.component';
import { ProductAdjustmentsClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-product-adjustment-list',
  templateUrl: './product-adjustment-list.component.html',
  styleUrl: './product-adjustment-list.component.scss',
  providers: [ProductAdjustmentsClient]
})
export class ProductAdjustmentListComponent {
  detailComponent = ProductAdjustmentDetailComponent;
  pageId = 'f77dbbb4-0ac4-491c-f153-08dcfa454f42'

  entityClient: ProductAdjustmentsClient = inject(ProductAdjustmentsClient);
}
