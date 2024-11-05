import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { GetPaymentListBySaleIdQuery, SaleModel, SalePaymentModel, SalePaymentsClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { SalePaymentDetailComponent } from '../sale-payment-detail/sale-payment-detail.component';
import { CommonConstants } from 'src/app/core/contants/common';
import { ConfirmDialogService } from 'src/app/shared/services/confirm-dialog.service';

interface Column {
  field: string;
  header: string;
}

@Component({
  selector: 'app-sale-payment-list',
  templateUrl: './sale-payment-list.component.html',
  styleUrl: './sale-payment-list.component.scss',
  providers: [SalePaymentsClient, ConfirmDialogService]
})
export class SalePaymentListComponent {
  saleModel: SaleModel;

  payments!: SalePaymentModel[];
  cols!: Column[];

  actionDropdownOptions: MenuItem[];

  // isUpdateSucceeded: boolean = false;
  // isDeleteSucceeded: boolean = false;

  constructor(private customDialogService: CustomDialogService,
    private entityClient: SalePaymentsClient,
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
    this.saleModel = this.customDialogService.getConfigData();

    this.cols = [
      // { field: 'paymentDate', header: 'Payment Date' },
      { field: 'paymentDateString', header: 'Payment Date' },
      { field: 'payingAmount', header: 'Payment' },
      { field: 'paymentTypeName', header: 'Payment Type' },
      { field: 'note', header: 'Payment Note' },
      { field: 'createdBy', header: 'Created By' }
    ];

    this.getList(this.saleModel.id)
  }

  onhandleMenuClick(event) {
    if (event?.menuItem?.id === 'edit' && event?.data?.id && event?.data?.id !== CommonConstants.EmptyGuid) {
      this.update(event.data)
    } else if (event?.menuItem?.id === 'delete') {
      this.delete(event.data)
    }
  }

  getList(saleId: string) {
    const query = new GetPaymentListBySaleIdQuery();
    query.saleId = saleId;
    this.entityClient.getAllBySaleId(query).subscribe({
      next: (res: any) => {
        this.payments = res;
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  update(data: SalePaymentModel) {
    const updatePaymentDialogRef = this.customDialogService.openDialog<{ id: string, salePayment: SalePaymentModel }>(
      SalePaymentDetailComponent,
      { id: data.id, salePayment: data },
      'Edit Payment'
    );

    updatePaymentDialogRef.onClose.subscribe({
      next: (updateSucceeded) => {
        // this.isUpdateSucceeded = updateSucceeded;
        if (updateSucceeded) {
          // Close the current Payment List dialog after update succeeds
          this.getList(data.saleId);
        }
      },
      error: (error) => { console.error('Error during dialog close:', error); },
      complete: () => { console.log('Dialog closed'); }
    });
  }

  delete(data: SalePaymentModel) {

    this.confirmDialogService.confirm(`Do you confirm?`).subscribe((confirmed) => {
      if (confirmed) {
        this.entityClient.delete(data.id).subscribe({
          next: () => {
            this.toast.deleted();
            // this.isDeleteSucceeded = true;
            this.getList(data.saleId);
          },
          error: (error) => {
            this.toast.showError(CommonUtils.getErrorMessage(error));
          }
        });
      }
    });

  }


}
