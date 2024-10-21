import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CommonConstants } from 'src/app/core/contants/common';
import { SaleModel, SalePaymentModel, SalePaymentsClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-sale-payment-detail',
  templateUrl: './sale-payment-detail.component.html',
  styleUrl: './sale-payment-detail.component.scss',
  providers: [SalePaymentsClient]
})
export class SalePaymentDetailComponent implements OnInit {

  saleModel: SaleModel;
  form: FormGroup;
  optionsDataSources: any;
  id: null;

  get f(){
    return this.form.controls;
  }

  constructor(private customDialogService: CustomDialogService,
    private fb: FormBuilder,
    private entityClient: SalePaymentsClient,
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
      this.saleModel = configData.sale;
      this.getById(CommonConstants.EmptyGuid)
    }

  }

  
  getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: SalePaymentModel) => {
        this.optionsDataSources = res.optionsDataSources;
        if(id && id !== CommonConstants.EmptyGuid){
          this.form.patchValue({
            ...res
          });
        } else {
          this.form.patchValue({
            receivedAmount: this.saleModel?.dueAmount,
            payingAmount: this.saleModel?.dueAmount,
            saleId: this.saleModel?.id,
            paymentType: res.paymentType
          });
        }
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  onSubmit(){
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
      saleId: [null],
      receivedAmount: [''],
      payingAmount: [''],
      changeAmount: [0],
      paymentType: [null],
      note: [null]
    });
  }

}
