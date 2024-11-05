import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonConstants } from 'src/app/core/contants/common';
import { ProductTransferInfoModel, ProductTransfersClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-product-transfer-info-detail',
  templateUrl: './product-transfer-info-detail.component.html',
  styleUrl: './product-transfer-info-detail.component.scss',
  providers: [ProductTransfersClient]
})
export class ProductTransferInfoDetailComponent {
  id: string;
  item: ProductTransferInfoModel;

  constructor(private entityClient: ProductTransfersClient,
    private activatedRoute: ActivatedRoute,
    private customDialogService: CustomDialogService,
    private toast: ToastService
  ) {
    this.id = this.customDialogService.getConfigData();

    if (!this.id || this.id === CommonConstants.EmptyGuid) {
      this.toast.showError("Transfer record not found");
      return;
    }
    // this.activatedRoute.paramMap.subscribe(params => {
    //   this.id = params.get('id')
    // });
    this.getDetailById(this.id)
  }

  ngOnInit(): void {

  }

  getDetailById(id: string) {
    this.entityClient.getDetail(id).subscribe({
      next: (res: any) => {
        this.item = res;
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

}
