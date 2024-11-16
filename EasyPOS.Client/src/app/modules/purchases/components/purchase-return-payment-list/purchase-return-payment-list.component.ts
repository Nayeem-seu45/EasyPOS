import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { GetPaymentListByPurchaseReturnIdQuery, PurchaseReturnModel, PurchaseReturnPaymentModel, PurchaseReturnPaymentsClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { CommonConstants } from 'src/app/core/contants/common';
import { ConfirmDialogService } from 'src/app/shared/services/confirm-dialog.service';
import { PurchaseReturnPaymentDetailComponent } from '../purchase-return-payment-detail/purchase-return-payment-detail.component';

interface Column {
  field: string;
  header: string;
}

@Component({
  selector: 'app-purchase-return-payment-list',
  templateUrl: './purchase-return-payment-list.component.html',
  styleUrl: './purchase-return-payment-list.component.scss',
  providers: [PurchaseReturnPaymentsClient, ConfirmDialogService]
})
export class PurchaseReturnPaymentListComponent {
  purchaseReturnModel: PurchaseReturnModel;

  payments!: PurchaseReturnPaymentModel[];
  cols!: Column[];

  actionDropdownOptions: MenuItem[];

  // isUpdateSucceeded: boolean = false;
  // isDeleteSucceeded: boolean = false;

  constructor(private customDialogService: CustomDialogService,
    private entityClient: PurchaseReturnPaymentsClient,
    private toast: ToastService,
    private confirmDialogService: ConfirmDialogService
  ) {

    this.actionDropdownOptions = [
      {
        label: 'Edit',
        icon: 'pi pi-pen-to-square',
        menuStyle: 'primary',
        id: 'edit'
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        menuStyle: 'danger',
        id: 'delete'
      },
    ];
  }

  ngOnInit(): void {
    this.purchaseReturnModel = this.customDialogService.getConfigData();

    this.cols = [
      // { field: 'paymentDate', header: 'Payment Date' },
      { field: 'paymentDateString', header: 'Payment Date' },
      { field: 'payingAmount', header: 'Payment' },
      { field: 'paymentTypeName', header: 'Payment Type' },
      { field: 'createdBy', header: 'Created By' },
      { field: 'note', header: 'Payment Note' },
    ];

    this.getList(this.purchaseReturnModel.id)
  }

  // onhandleMenuClick(event) {
  //   if (event?.menuItem?.id === 'edit' && event?.data?.id && event?.data?.id !== CommonConstants.EmptyGuid) {
  //     this.update(event.data)
  //   } else if (event?.menuItem?.id === 'delete') {
  //     this.delete(event.data)
  //   }
  // }

  getList(purchaseReturnId: string) {
    const query = new GetPaymentListByPurchaseReturnIdQuery();
    query.purchaseReturnId = purchaseReturnId;
    this.entityClient.getAllByPurchaseReturnId(query).subscribe({
      next: (res: any) => {
        this.payments = res;
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  update(data: PurchaseReturnPaymentModel) {
    const updatePaymentDialogRef = this.customDialogService.openDialog<{ id: string, purchaseReturnPayment: PurchaseReturnPaymentModel }>(
      PurchaseReturnPaymentDetailComponent,
      { id: data.id, purchaseReturnPayment: data },
      'Edit Payment'
    );

    updatePaymentDialogRef.onClose.subscribe({
      next: (updateSucceeded) => {
        // this.isUpdateSucceeded = updateSucceeded;
        if (updateSucceeded) {
          // Close the current Payment List dialog after update succeeds
          this.getList(data.purchaseReturnId);
        }
      },
      error: (error) => { console.error('Error during dialog close:', error); },
      complete: () => { console.log('Dialog closed'); }
    });
  }

  delete(data: PurchaseReturnPaymentModel) {

    this.confirmDialogService.confirm(`Do you confirm?`).subscribe((confirmed) => {
      if (confirmed) {
        this.entityClient.delete(data.id).subscribe({
          next: () => {
            this.toast.deleted();
            // this.isDeleteSucceeded = true;
            this.getList(data.purchaseReturnId);
          },
          error: (error) => {
            this.toast.showError(CommonUtils.getErrorMessage(error));
          }
        });
      }
    });

  }


}
