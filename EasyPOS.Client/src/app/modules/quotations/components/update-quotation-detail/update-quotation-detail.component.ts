import { Component, inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DiscountType, QuotationDetailModel, TaxMethod } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-update-quotation-detail',
  templateUrl: './update-quotation-detail.component.html',
  styleUrl: './update-quotation-detail.component.scss'
})
export class UpdateQuotationDetailComponent {
  form: FormGroup;
  item: QuotationDetailModel;
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
    const data = this.customDialogService.getConfigData<{quotationDetail: QuotationDetailModel, optionsDataSources: any}>();
    this.optionsDataSources = data.optionsDataSources;
    this.discountTypes = CommonUtils.enumToArray(DiscountType);
    this.taxMethods = CommonUtils.enumToArray(TaxMethod);
    this.initializeFormGroup(data.quotationDetail);
  }

  onDiscountTypeChange(){
    this.f['discountRate'].setValue(null, { emitEvent: false });
    this.f['productUnitDiscount'].setValue(0, { emitEvent: false });
  }

  private initializeFormGroup(item: QuotationDetailModel){
    this.form = this.fb.group({
      quantity: [item.quantity],
      productUnitPrice: [item.productUnitPrice],
      productUnitId: [item.productUnitId],
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

  protected onFormSubmit() {
    if (this.form.invalid) {
      return;
    }

    this.update();
  }

  protected update() {
    const updatedQuotationDetail = { ...this.form.value };
    this.customDialogService.closeWithData<QuotationDetailModel>(true, updatedQuotationDetail)
  }
}
