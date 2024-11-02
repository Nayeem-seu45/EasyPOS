import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonConstants } from 'src/app/core/contants/common';
import { SaleInfoModel, SalesClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-sale-info-detail',
  templateUrl: './sale-info-detail.component.html',
  styleUrl: './sale-info-detail.component.scss',
  providers: [SalesClient]
})
export class SaleInfoDetailComponent {
  id: string;
  item: SaleInfoModel;

  constructor(private entityClient: SalesClient,
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
