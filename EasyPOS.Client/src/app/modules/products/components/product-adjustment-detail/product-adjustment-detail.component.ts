import { Component, inject } from '@angular/core';
import { ProductAdjAction, ProductAdjustmentModel, ProductAdjustmentsClient, ProductSelectListModel } from 'src/app/modules/generated-clients/api-service';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { CommonConstants } from 'src/app/core/contants/common';
import { ActivatedRoute } from '@angular/router';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { DatePipe } from '@angular/common';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-product-adjustment-detail',
  templateUrl: './product-adjustment-detail.component.html',
  styleUrl: './product-adjustment-detail.component.scss',
  providers: [ProductAdjustmentsClient, DatePipe]
})
export class ProductAdjustmentDetailComponent {

  id: string = '';
  optionsDataSources = {};
  form: FormGroup;
  item: ProductAdjustmentModel;

  selectedProduct: ProductSelectListModel;
  actionTypeSelectList: any;

  get f(){
    return this.form.controls;
  }

  get productAdjustmentDetails(): FormArray{
    return this.form.get('productAdjustmentDetails') as FormArray;
  }

  protected toast: ToastService = inject(ToastService);
  protected fb: FormBuilder = inject(FormBuilder);
  protected datePipe: DatePipe = inject(DatePipe);

  constructor(private entityClient: ProductAdjustmentsClient,
  private activatedRoute: ActivatedRoute
  ){
  }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id')
    });

    this.actionTypeSelectList = CommonUtils.enumToArray(ProductAdjAction)

    this.initializeFormGroup();
    this.getById(this.id || CommonConstants.EmptyGuid)
  }

  onProductSelect(){
    const newAdjustProduct = this.createProductAdjustmentDetail();

    console.log(this.selectedProduct)
    newAdjustProduct.patchValue({
      productId: this.selectedProduct.id,
      productName: this.selectedProduct.name,
      productCode: this.selectedProduct.code,
      unitCost: this.selectedProduct.costPrice,
      stock: 0,
    });

    this.productAdjustmentDetails.push(newAdjustProduct);

    this.selectedProduct = null;
  }

  onSubmit() {
    if (this.form.invalid) {
      this.toast.showError('Form is invalid.');
      return;
    }

    if (!this.id || this.id === CommonConstants.EmptyGuid) {
      this.save();
    } else {
      this.update();
    }
  }


  getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: any) => {
        if (id !== CommonConstants.EmptyGuid) {
          this.item = res;
          this.item.productAdjustmentDetails?.forEach(() => {
            this.productAdjustmentDetails.push(this.createProductAdjustmentDetail());
          });
        }
        this.optionsDataSources = res.optionsDataSources;
        this.form.patchValue(this.item)
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  save() {
    const createCommand = { ...this.form.value };
    createCommand.adjDate = this.datePipe.transform(createCommand.adjDate, 'yyyy-MM-dd');
    console.log(createCommand)
    this.entityClient.create(createCommand).subscribe({
      next: () => {
        this.toast.created();
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  update() {
    const updateCommand = { ...this.form.value };
    updateCommand.purchaseDate = this.datePipe.transform(updateCommand.purchaseDate, 'yyyy-MM-dd');
    this.entityClient.update(updateCommand).subscribe({
      next: () => {
        this.toast.updated();
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  removeAdjDetail(index: number){
    const adjDetail = this.productAdjustmentDetails.at(index).value;

    this.productAdjustmentDetails.removeAt(index);

    this.deleteAdjustProduct(adjDetail.id)
  }


  private deleteAdjustProduct(id: string) {
    if (!id) {
      return;
    }

    this.entityClient.deleteAdjDetail(id).subscribe({
      next: () => {
        console.log('delete detail')
      }, error: (error) => {
        console.log(error)
      }
    });
  }

  onFileUpload(fileUrl) {
    this.form.get('attachmentUrl').setValue(fileUrl[0]);
  }

 initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      warehouseId: [null],
      attachmentUrl: [null],
      note: [null],
      adjDate: [null],
      productAdjustmentDetails: this.fb.array([])
    });
  }

  createProductAdjustmentDetail(): FormGroup{
    return this.fb.group({
      id: [CommonConstants.EmptyGuid],
      productAdjustmentId: [CommonConstants.EmptyGuid],
      productId: [null],
      productName: [null],
      productCode: [null],
      unitCost: [0],
      quantity: [1],
      stock: [0],
      actionType: [ProductAdjAction.Subtraction] 
    });
  }

}
