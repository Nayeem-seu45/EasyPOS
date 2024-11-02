import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonConstants } from 'src/app/core/contants/common';
import { QuotationInfoModel, QuotationsClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-quotation-info-detail',
  templateUrl: './quotation-info-detail.component.html',
  styleUrl: './quotation-info-detail.component.scss',
  providers: [QuotationsClient]
})
export class QuotationInfoDetailComponent {
  id: string;
  item: QuotationInfoModel;

  constructor(private entityClient: QuotationsClient,
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
        console.log(res)
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }
}
