import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CommonConstants } from 'src/app/core/contants/common';
import { SaleReturnModel, SaleReturnPaymentModel, SaleReturnPaymentsClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-sale-return-payment-detail',
  templateUrl: './sale-return-payment-detail.component.html',
  styleUrl: './sale-return-payment-detail.component.scss',
  providers: [SaleReturnPaymentsClient]
})
export class SaleReturnPaymentDetailComponent implements OnInit {

  saleReturnModel: SaleReturnModel;
  form: FormGroup;
  optionsDataSources: any;
  id: null;

  get f(){
    return this.form.controls;
  }

  constructor(private customDialogService: CustomDialogService,
    private fb: FormBuilder,
    private entityClient: SaleReturnPaymentsClient,
    private toast: ToastService
  ){
    this.initializeFormGroup();
  }

  ngOnInit(): void {
    const configData = this.customDialogService.getConfigData<any>();
    console.log(configData)
    this.id = configData.id;
    if(this.id && this.id !== CommonConstants.EmptyGuid){
      this.getById(this.id)
    } else {
      this.saleReturnModel = configData.saleReturn;
      this.getById(CommonConstants.EmptyGuid)
    }

  }

  
  getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: SaleReturnPaymentModel) => {
        this.optionsDataSources = res.optionsDataSources;
        if(id && id !== CommonConstants.EmptyGuid){
          this.form.patchValue({
            ...res
          });
        } else {
          this.form.patchValue({
            receivedAmount: this.saleReturnModel?.dueAmount,
            payingAmount: this.saleReturnModel?.dueAmount,
            saleReturnId: this.saleReturnModel?.id,
            paymentType: res.paymentType
          });
        }
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  onFormSubmit(){
    if(this.id && this.id !== CommonConstants.EmptyGuid){
      this.update();
    } else {
      this.create();
    }
  }

  cancel(){
    this.customDialogService.close(false);
  }

  updateChangeAmount(){
    const receivedAmount = this.form.get('receivedAmount').value;
    const payingAmount = this.form.get('payingAmount').value;
    this.form.get('changeAmount').setValue(receivedAmount - payingAmount);
  }

  private create() {
    const createCommand = { ...this.form.value };
    this.entityClient.create(createCommand).subscribe({
      next: () => {
        this.toast.created();
        this.customDialogService.close(true);
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  private update() {
    const updateCommand = { ...this.form.value };
    this.entityClient.update(updateCommand).subscribe({
      next: () => {
        this.toast.updated();
        this.customDialogService.closeLastDialog(true);
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  private initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      saleReturnId: [null],
      receivedAmount: [''],
      payingAmount: [''],
      changeAmount: [0],
      paymentType: [null],
      note: [null]
    });
  }

}
