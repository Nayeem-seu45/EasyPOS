import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonConstants } from 'src/app/core/contants/common';
import { PurchaseInfoModel, PurchasesClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-purchase-info-detail',
  templateUrl: './purchase-info-detail.component.html',
  styleUrl: './purchase-info-detail.component.scss',
  providers: [PurchasesClient]
})
export class PurchaseInfoDetailComponent {
  id: string;
  item: PurchaseInfoModel;

  constructor(private entityClient: PurchasesClient,
    private activatedRoute: ActivatedRoute,
    private customDialogService: CustomDialogService,
    private toast: ToastService
  ) { 

    this.activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id')
    });

    if (this.id && this.id != CommonConstants.EmptyGuid) {
      this.getDetailById(this.id)
    }
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
