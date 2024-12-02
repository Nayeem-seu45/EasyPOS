import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CommonConstants } from 'src/app/core/contants/common';
import { DiscountType, GetProductSearchInStockSelectListQuery, ProductsClient, ProductSelectListModel, ProductTransferDetailModel, ProductTransferModel, ProductTransfersClient, TaxMethod } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { Subject, Subscription } from 'rxjs';
import { UpdateProductTransferOrderDetailComponent } from '../update-product-transfer-order-detail/update-product-transfer-order-detail.component';
import { isArraysEqual } from '@fullcalendar/core/internal';

@Component({
  selector: 'app-product-transfer-detail',
  templateUrl: './product-transfer-detail.component.html',
  styleUrl: './product-transfer-detail.component.scss',
  providers: [ProductTransfersClient, DatePipe, ProductsClient]
})
export class ProductTransferDetailComponent implements OnInit {
  emptyGuid = CommonConstants.EmptyGuid;
  id: string = '';
  optionsDataSources = {};
  form: FormGroup;
  item: ProductTransferModel;

  // Table footer section
  totalQuantity: number = 0;
  totalDiscount: number = 0;
  totalTaxAmount: number = 0;
  subTotal: number = 0;

  // Grand total Section
  totalItems: string = '0';
  orderTaxAmount: number = 0;
  orderDiscountAmount: number = 0;
  shippingCostAmount: number = 0;
  grandTotalAmount: number = 0;

  selectedProduct: any;
  discountTypes: { id: number, name: string }[] = [];
  DiscountType = DiscountType;

  private closeDialogsubscription: Subscription;

  get isEdit() {
    return this.id && this.id !== CommonConstants.EmptyGuid;
  }

  get productTransferDetails(): FormArray {
    return this.form.get('productTransferDetails') as FormArray;
  }

  get f() {
    return this.form.controls;
  }

  protected toast: ToastService = inject(ToastService);
  protected fb: FormBuilder = inject(FormBuilder);
  protected datePipe: DatePipe = inject(DatePipe);

  constructor(private entityClient: ProductTransfersClient,
    private activatedRoute: ActivatedRoute,
    private customDialogService: CustomDialogService
  ) { }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id')
    });
    this.discountTypes = CommonUtils.enumToArray(DiscountType);

    this.getById(this.id || this.emptyGuid)
    this.initializeFormGroup();
  }

  ngOnDestroy() {
    // Unsubscribe when the component is destroyed to avoid memory leaks
    if (this.closeDialogsubscription) {
      this.closeDialogsubscription.unsubscribe();
    }
  }

  //#region AutoComplete Search
  showWarehouseValidationMsg: boolean = false;

  onWarehouseChange(event: any) {
    if (event && event !== CommonConstants.EmptyGuid) {
      this.showWarehouseValidationMsg = false; // Hide the validation message when a warehouse is selected
    }
  }

  getWarehouseValidation(event: boolean){
    this.showWarehouseValidationMsg = event;
  }

  onProductSelect(selectedEvent: any) {
    const selectedProduct = selectedEvent.value;
    if (selectedProduct.value) {
      this.addProductToProductTransferDetails(selectedProduct.value);
    }
  }

  onExactMatchProduct(product: ProductSelectListModel) {
    this.addProductToProductTransferDetails(product);
  }

  //#endregion

  // #region CRUDS

  onFormSubmit() {
    if (this.form.invalid) {
      this.toast.showError('Form is invalid.');
      return;
    }

    if (!this.id || this.id === this.emptyGuid) {
      this.save();
    } else {
      this.update();
    }
  }

  getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: any) => {
        if (id !== this.emptyGuid) {
          this.item = res;
          this.item.productTransferDetails?.forEach(() => {
            this.productTransferDetails.push(this.addProductTransferDetailFormGroup());
          });
        }
        this.optionsDataSources = res.optionsDataSources;
        this.form.patchValue(this.item);
        this.calculateFooterSection();
        this.calculateGrandTotal();

        console.log(this.form.value)
        console.log(res)
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  save() {
    const createCommand = { ...this.form.value };
    createCommand.transferDate = this.datePipe.transform(createCommand.transferDate, 'yyyy-MM-dd');
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
    updateCommand.transferDate = this.datePipe.transform(updateCommand.transferDate, 'yyyy-MM-dd');
    console.log(updateCommand.transferDate)
    this.entityClient.update(updateCommand).subscribe({
      next: () => {
        this.toast.updated();
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  initializeFormGroup(): void {
    this.form = this.fb.group({
      id: [null],
      transferDate: [null],
      referenceNo: [null],
      fromWarehouseId: [null],
      toWarehouseId: [null],
      transferStatusId: [null],
      attachmentUrl: [null],
      subTotal: [0],
      taxRate: [0],
      taxAmount: [0],
      discountType: [DiscountType.Fixed],
      discountRate: [0],
      discountAmount: [0],
      shippingCost: [0],
      grandTotal: [0],
      note: [null],
      productTransferDetails: this.fb.array([])
    });
  }

  private addProductTransferDetailFormGroup(): FormGroup {
    return this.fb.group({
      id: [CommonConstants.EmptyGuid],
      productId: [null],
      productTransferId: [CommonConstants.EmptyGuid],
      productCode: [null],
      productName: [null],
      productUnitCost: [0],
      productUnitPrice: [0],
      productUnitId: [null],
      productUnit: [0],
      productUnitDiscount: [0],
      quantity: [1],
      expiredDate: [null],
      batchNo: [null],
      netUnitCost: [0],
      discountType: [DiscountType.Fixed],
      discountRate: [0],
      discountAmount: [0],
      taxMethod: [TaxMethod.Exclusive],
      taxRate: [0],
      taxAmount: [0],
      totalPrice: [0],
      remarks: ['']
    });
  }

  // #endregion

  // #region Add or Update ProductTransferDetail


  // onProductSelect(selectedProduct: ProductSelectListModel) {
  //   if (selectedProduct) {
  //     this.addProductToProductTransferDetails(selectedProduct);
  //   }
  // }

  removeProductTransferDetail(index: number) {

    const product = this.productTransferDetails.at(index).value;

    this.productTransferDetails.removeAt(index);

    this.deleteProductTransferDetail(product?.id)

    this.calculateGrandTotal();
  }

  private addProductToProductTransferDetails(product: ProductSelectListModel) {

    const isAlreadyExisted = this.increaseQuantityIfProductAlreadyExist(product);

    if (isAlreadyExisted) return;

    const productFormGroup = this.addProductTransferDetailFormGroup();
    const quantity = 1;
    // const totalDiscountAmount = (product.discountAmount || 0) * quantity;

    // Set the values in the form group
    productFormGroup.patchValue({
      productId: product.id,
      productCode: product.code,
      productName: product.name,
      productUnitCost: product.costPrice,
      productUnitPrice: product.salePrice,
      productUnitId: product.saleUnit,
      productUnitDiscount: product.discountAmount || 0,
      quantity: quantity,
      discountType: product.discountType || DiscountType.Fixed,
      discountRate: product.discountRate || 0,
      // discountAmount: totalDiscountAmount,
      taxMethod: product.taxMethod,
      taxRate: product.taxRate || 0,
    });

    this.productTransferDetails.push(productFormGroup);

    this.calculateTaxAndTotalPrice(this.productTransferDetails.length - 1, productFormGroup.value);
  }

  private increaseQuantityIfProductAlreadyExist(product: ProductSelectListModel): boolean {
    // Access the controls of the FormArray
    const existingProductFormGroup = this.productTransferDetails.controls.find(
      (formGroup) =>
        (formGroup as FormGroup).value.productId === product.id
    ) as FormGroup;
  
    if (existingProductFormGroup) {
      // Increase quantity
      const updatedQuantity = existingProductFormGroup.value.quantity + 1;
  
      // Update the FormGroup with the new quantity
      existingProductFormGroup.patchValue({ quantity: updatedQuantity });
  
      // Recalculate tax and total price
      const index = this.productTransferDetails.controls.indexOf(existingProductFormGroup);
      this.calculateTaxAndTotalPrice(index, existingProductFormGroup.value);
  
      // this.calculateGrandTotal();
      return true;
    }
  
    return false;
  }

  private deleteProductTransferDetail(id: string) {
    if (!id) {
      return;
    }

    this.entityClient.deleteProductTransferDetail(id).subscribe({
      next: () => {
        console.log('delete detail')
      }, error: (error) => {
        console.log(error)
      }
    });
  }

  onItemPropsChange(index: number) {
    const productTransferDetailFormGroup = this.productTransferDetails.at(index) as FormGroup;
    const productTransferDetail = productTransferDetailFormGroup.value;
    this.calculateTaxAndTotalPrice(index, productTransferDetail);
  }


  private calculateTaxAndTotalPrice(index: number, productTransferDetail: ProductTransferDetailModel) {
    let netUnitCost: number;
    let taxAmount: number;
    let totalPrice: number;
    let productUnitDiscount: number = 0;
    let totalDiscountAmount: number = 0;

    if (productTransferDetail.discountType === DiscountType.Fixed) {
      productUnitDiscount = productTransferDetail.productUnitDiscount;
    } else if (productTransferDetail.discountType === DiscountType.Percentage) {
      productUnitDiscount = (productTransferDetail.productUnitPrice * productTransferDetail.discountRate) / 100;
    }

    totalDiscountAmount = parseFloat((productUnitDiscount * productTransferDetail.quantity).toFixed(2));
    const taxRateDecimal = productTransferDetail.taxRate / 100;

    if (productTransferDetail.taxMethod === TaxMethod.Exclusive) {
      netUnitCost = productTransferDetail.productUnitCost - (productUnitDiscount || 0);
      const taxableTotalPrice = netUnitCost * productTransferDetail.quantity;
      taxAmount = taxableTotalPrice * taxRateDecimal;
      totalPrice = taxableTotalPrice + taxAmount;
    }
    else if (productTransferDetail.taxMethod === TaxMethod.Inclusive) {
      const priceAfterDiscount = productTransferDetail.productUnitCost - (productUnitDiscount || 0);
      const taxRateFactor = 1 + taxRateDecimal;
      netUnitCost = priceAfterDiscount / taxRateFactor;
      taxAmount = (netUnitCost * productTransferDetail.quantity) * (productTransferDetail.taxRate / 100);
      totalPrice = (netUnitCost * productTransferDetail.quantity) + taxAmount;
    }

    this.productTransferDetails.at(index).patchValue({
      productUnitDiscount: parseFloat(productUnitDiscount.toFixed(2)),
      netUnitCost: parseFloat(netUnitCost.toFixed(2)),
      discountAmount: parseFloat(totalDiscountAmount.toFixed(2)),
      taxAmount: parseFloat(taxAmount.toFixed(2)),
      totalPrice: parseFloat(totalPrice.toFixed(2))
    }, { emitEvent: false });

    this.calculateFooterSection();
    this.calculateGrandTotal();
  }

  updateProductTransferDetail(index: number) {
    console.log(index)

    const productTransferDetailFormGroup = this.productTransferDetails.at(index) as FormGroup;
    const productTransferDetail = productTransferDetailFormGroup.value;
    this.customDialogService.open<{ productTransferDetail: ProductTransferDetailModel; optionsDataSources: any }>(
      UpdateProductTransferOrderDetailComponent,
      { productTransferDetail: productTransferDetail, optionsDataSources: this.optionsDataSources },
      productTransferDetail.productName
    )
      .subscribe((succeeded) => {
        if (succeeded) {

          if (this.closeDialogsubscription) {
            this.closeDialogsubscription.unsubscribe();
          }

          this.closeDialogsubscription = this.customDialogService.closeDataSubject.subscribe((updatedProductTransferDetail: ProductTransferDetailModel) => {

            productTransferDetailFormGroup.patchValue({
              productUnitCost: updatedProductTransferDetail.productUnitCost,
              productUnitId: updatedProductTransferDetail.productUnitId,
              taxMethod: updatedProductTransferDetail.taxMethod,
              taxRate: updatedProductTransferDetail.taxRate,
              discountType: updatedProductTransferDetail.discountType,
              discountRate: updatedProductTransferDetail.discountRate,
              productUnitDiscount: updatedProductTransferDetail.productUnitDiscount,
            }, { emitEvent: false });
            const updateProductTransferDetailValue = productTransferDetailFormGroup.value;
            this.calculateTaxAndTotalPrice(index, updateProductTransferDetailValue);

          });
        }
      });

  }

  private openDialog(index: number, productTransferDetail: ProductTransferDetailModel) {
    this.customDialogService.open<{ productTransferDetail: ProductTransferDetailModel; optionsDataSources: any }>(
      UpdateProductTransferOrderDetailComponent,
      { productTransferDetail: productTransferDetail, optionsDataSources: this.optionsDataSources },
      productTransferDetail.productName
    )
      .subscribe((succeeded) => {
        if (succeeded) {
          this.closeDialogsubscription = this.customDialogService.closeDataSubject.subscribe((updatedProductTransferDetail: ProductTransferDetailModel) => {
            const productTransferDetailFormGroup = this.productTransferDetails.at(index) as FormGroup;


            productTransferDetailFormGroup.patchValue({
              productUnitCost: updatedProductTransferDetail.productUnitCost,
              productUnitId: updatedProductTransferDetail.productUnitId,
              taxMethod: updatedProductTransferDetail.taxMethod,
              taxRate: updatedProductTransferDetail.taxRate,
              discountType: updatedProductTransferDetail.discountType,
              discountRate: updatedProductTransferDetail.discountRate,
              productUnitDiscount: updatedProductTransferDetail.productUnitDiscount,
            }, { emitEvent: false });
            const updateProductTransferDetailValue = productTransferDetailFormGroup.value;
            this.calculateTaxAndTotalPrice(index, updateProductTransferDetailValue);

          });
        }
      });
  }


  // #endregion

  // #region ProductTransferOrder Footer

  calculateFooterSection() {
    this.totalQuantity = this.productTransferDetails.controls.reduce((acc, curr) => acc + (curr.get('quantity').value || 0), 0);
    this.totalDiscount = parseFloat(this.productTransferDetails.controls.reduce((acc, curr) => acc + (curr.get('discountAmount').value || 0), 0).toFixed(2));
    this.totalTaxAmount = parseFloat(this.productTransferDetails.controls.reduce((acc, curr) => acc + (curr.get('taxAmount').value || 0), 0).toFixed(2));
    this.subTotal = parseFloat(this.productTransferDetails.controls.reduce((acc, curr) => acc + (curr.get('totalPrice').value || 0), 0).toFixed(2));
    this.form.get('subTotal').setValue(this.subTotal, { emitEvent: false })
  }

  // #endregion

  // #region Grand Total Section

  onOrderTaxChange() {
    this.calculateGrandTotal();
  }

  onDiscountTypeChange() {
    this.f['discountRate'].setValue(null, { emitEvent: false });
    this.f['discountAmount'].setValue(0, { emitEvent: false });

    this.calculateGrandTotal();
  }

  onOrderDiscountChange() {
    this.calculateGrandTotal();
  }

  onShippingCostChange() {
    this.calculateGrandTotal();
  }

  private calculateGrandTotal() {
    const taxRate = parseFloat(this.f['taxRate'].value || 0);

    this.orderDiscountAmount = parseFloat(this.f['discountAmount'].value || 0);
    this.shippingCostAmount = parseFloat(this.f['shippingCost'].value || 0);

    const taxableAmount = this.subTotal - this.orderDiscountAmount;
    this.orderTaxAmount = parseFloat(((taxableAmount * taxRate) / 100).toFixed(2));

    const totalProducts = this.productTransferDetails.length;
    this.totalItems = totalProducts > 0 ? `${this.productTransferDetails.length}(${this.totalQuantity})` : '0';

    // Calculate grand total
    this.grandTotalAmount = parseFloat(
      (this.subTotal + this.orderTaxAmount + this.shippingCostAmount - this.orderDiscountAmount).toFixed(2)
    );

    this.f['taxAmount'].setValue(this.orderTaxAmount, { emitEvent: false });
    this.f['grandTotal'].setValue(this.grandTotalAmount, { emitEvent: false });
  }

  // #endregion


  onFileUpload(fileUrl) {

  }

}
