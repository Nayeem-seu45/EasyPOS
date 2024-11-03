import { Component, inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DiscountType, ProductTransferDetailModel, TaxMethod } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-update-product-transfer-detail',
  templateUrl: './update-product-transfer-order-detail.component.html',
  styleUrl: './update-product-transfer-order-detail.component.scss'
})
export class UpdateProductTransferOrderDetailComponent {
  form: FormGroup;
  item: ProductTransferDetailModel;
  optionsDataSources: any;

  taxMethods: { id: number, name: string }[] = [];
  discountTypes: { id: number, name: string }[] = [];
  DiscountType = DiscountType;

  protected get f() {
    return this.form.controls;
  }
  
  protected customDialogService: CustomDialogService = inject(CustomDialogService)
  protected fb: FormBuilder = inject(FormBuilder);
  
  constructor() {}

  ngOnInit() {
    const data = this.customDialogService.getConfigData<{productTransferDetail: ProductTransferDetailModel, optionsDataSources: any}>();
    this.optionsDataSources = data.optionsDataSources;
    this.discountTypes = CommonUtils.enumToArray(DiscountType);
    this.taxMethods = CommonUtils.enumToArray(TaxMethod);
    this.initializeFormGroup(data.productTransferDetail);
  }

  onDiscountTypeChange(){
    this.f['discountRate'].setValue(null, { emitEvent: false });
    this.f['productUnitDiscount'].setValue(0, { emitEvent: false });
  }

  private initializeFormGroup(item: ProductTransferDetailModel){
    this.form = this.fb.group({
      quantity: [item.quantity],
      productUnitCost: [item.productUnitCost],
      productUnitId: [item.productUnit],
      taxMethod: [item.taxMethod],
      taxRate: [item.taxRate],
      discountType: [item.discountType],
      discountRate: [item.discountRate],
      productUnitDiscount: [item.productUnitDiscount]
    });
  }

  protected cancel() {
    this.customDialogService.close(false);
  }

  protected onSubmit() {
    if (this.form.invalid) {
      return;
    }

    this.update();
  }

  protected update() {
    const updatedProductTransferDetail = { ...this.form.value };
    this.customDialogService.closeWithData<ProductTransferDetailModel>(true, updatedProductTransferDetail)
  }

  protected getErrorMessage(error: any): string {
    return error?.errors?.[0]?.description || 'An unexpected error occurred';
  }
}
