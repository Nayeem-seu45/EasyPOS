import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { ProductAdjAction, ProductAdjustmentsClient, ProductSelectListModel } from 'src/app/modules/generated-clients/api-service';
import { FormArray, FormGroup } from '@angular/forms';
import { CommonConstants } from 'src/app/core/contants/common';

@Component({
  selector: 'app-product-adjustment-detail',
  templateUrl: './product-adjustment-detail.component.html',
  styleUrl: './product-adjustment-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: ProductAdjustmentsClient}]
})
export class ProductAdjustmentDetailComponent extends BaseDetailComponent {

  selectedProduct: ProductSelectListModel;

  get productAdjustmentIdDetails(): FormArray{
    return this.form.get('productAdjustmentIdDetails') as FormArray;
  }

  constructor(@Inject(ENTITY_CLIENT) entityClient: ProductAdjustmentsClient){
    super(entityClient)
  }

  onProductSelect(){
    const newAdjustProduct = this.createProductAdjustmentDetail();

    newAdjustProduct.patchValue({
      productId: [this.selectedProduct.id],
      productName: [this.selectedProduct.name],
      productCode: [this.selectedProduct.code],
      unitCost: [this.selectedProduct.costPrice],
      stock: [0],
    });

    this.productAdjustmentIdDetails.push(newAdjustProduct);

    this.selectedProduct = null;
  }

  removeAdjDetail(index: number){
    this.productAdjustmentIdDetails.removeAt(index);
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      warehouseId: [null],
      attachmentUrl: [null],
      note: [null],
      adjDate: [null],
      productAdjustmentIdDetails: this.fb.array([])
    });
  }

  createProductAdjustmentDetail(): FormGroup{
    return this.fb.group({
      id: [CommonConstants.EmptyGuid],
      productAdjustmentId: [null],
      productId: [null],
      productName: [null],
      productCode: [null],
      unitCost: [0],
      quantity: [1],
      stock: [0],
      actionType: [ProductAdjAction.Subtraction] 
    });
  }

}
