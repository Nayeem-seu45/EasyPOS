import { DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CommonConstants } from 'src/app/core/contants/common';
import { DiscountType, ProductSelectListModel, PurchaseReturnDetailModel, PurchaseReturnModel, PurchaseReturnsClient, TaxMethod } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { Subscription } from 'rxjs';
import { UpdatePurchaseOrderDetailComponent } from '../update-purchase-order-detail/update-purchase-order-detail.component';

@Component({
  selector: 'app-purchase-return-detail',
  templateUrl: './purchase-return-detail.component.html',
  styleUrl: './purchase-return-detail.component.scss',
  providers: [PurchaseReturnsClient, DatePipe]
})
export class PurchaseReturnDetailComponent implements OnInit {
  id: string = '';
  purchaseId: string = '';
  optionsDataSources = {};
  form: FormGroup;
  item: PurchaseReturnModel;
  processFrom: string;

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

  discountTypes: { id: number, name: string }[] = [];
  DiscountType = DiscountType;

  private closeDialogsubscription: Subscription;

  get purchaseReturnDetails(): FormArray {
    return this.form.get('purchaseReturnDetails') as FormArray;
  }

  get f() {
    return this.form.controls;
  }

  protected toast: ToastService = inject(ToastService);
  protected fb: FormBuilder = inject(FormBuilder);
  protected datePipe: DatePipe = inject(DatePipe);

  constructor(private entityClient: PurchaseReturnsClient,
    private activatedRoute: ActivatedRoute,
    private customDialogService: CustomDialogService
  ) { }

  ngOnInit(): void {

    this.processFrom = this.activatedRoute.snapshot.data["processFrom"];
    this.purchaseId = this.activatedRoute.snapshot.data["purchaseId"];

    this.activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id')
      this.purchaseId = params.get('purchaseId')
    });

    this.discountTypes = CommonUtils.enumToArray(DiscountType);

    if (this.processFrom === 'return-list') {
      this.getById(this.id || CommonConstants.EmptyGuid)
    } else {
      this.getByPurchaseId(this.purchaseId || CommonConstants.EmptyGuid)
    }
    this.initializeFormGroup();
  }

  ngOnDestroy() {
    // Unsubscribe when the component is destroyed to avoid memory leaks
    if (this.closeDialogsubscription) {
      this.closeDialogsubscription.unsubscribe();
    }
  }

  // #region CRUDS

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
        this.item = res;
        this.item.purchaseReturnDetails?.forEach(() => {
          this.purchaseReturnDetails.push(this.addPurchaseReturnDetailFormGroup());
        });
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

  getByPurchaseId(purchaseId: string) {
    this.entityClient.getByPurchaseId(purchaseId).subscribe({
      next: (res: any) => {
        this.item = res;
        this.item.purchaseReturnDetails?.forEach(() => {
          this.purchaseReturnDetails.push(this.addPurchaseReturnDetailFormGroup());
        });
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
    createCommand.returnDate = this.datePipe.transform(createCommand.returnDate, 'yyyy-MM-dd');
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
    updateCommand.returnDate = this.datePipe.transform(updateCommand.returnDate, 'yyyy-MM-dd');
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
      purchaseId: [CommonConstants.EmptyGuid],
      returnDate: [null],
      referenceNo: [null],
      purchaseReferenceNo: [null],
      warehouseId: [null],
      supplierId: [null],
      returnStatusId: [null],
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
      purchaseReturnDetails: this.fb.array([])
    });
  }

  private addPurchaseReturnDetailFormGroup(): FormGroup {
    return this.fb.group({
      id: [CommonConstants.EmptyGuid],
      productId: [null],
      purchaseReturnId: [CommonConstants.EmptyGuid],
      productCode: [null],
      productName: [null],
      productUnitCost: [0],
      productUnitPrice: [0],
      productUnitId: [null],
      productUnit: [0],
      productUnitDiscount: [0],
      purchasedQuantity: [0],
      returnedQuantity: [1],
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

  // #region Add or Update PurchaseReturnDetail


  onProductSelect(selectedProduct: ProductSelectListModel) {
    if (selectedProduct) {
      this.addProductToPurchaseReturnDetails(selectedProduct);
    }
  }

  removeOrderDetail(index: number) {

    const product = this.purchaseReturnDetails.at(index).value;

    this.purchaseReturnDetails.removeAt(index);

    if(this.processFrom === 'return-list'){
      this.deletePurchaseReturnDetail(product?.id)
    }
    this.calculateGrandTotal();  

  }

  private addProductToPurchaseReturnDetails(product: ProductSelectListModel) {
    const productFormGroup = this.addPurchaseReturnDetailFormGroup();
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

    this.purchaseReturnDetails.push(productFormGroup);

    this.calculateTaxAndTotalPrice(this.purchaseReturnDetails.length - 1, productFormGroup.value);
  }

  private deletePurchaseReturnDetail(id: string) {
    if (!id) {
      return;
    }

    this.entityClient.deletePurchaseReturnDetail(id).subscribe({
      next: () => {
        console.log('delete detail')
      }, error: (error) => {
        console.log(error)
      }
    });
  }

  onItemPropsChange(index: number) {
    const purchaseReturnDetailFormGroup = this.purchaseReturnDetails.at(index) as FormGroup;
    const purchaseReturnDetail = purchaseReturnDetailFormGroup.value;
    this.calculateTaxAndTotalPrice(index, purchaseReturnDetail);
  }


  private calculateTaxAndTotalPrice(index: number, purchaseReturnDetail: PurchaseReturnDetailModel) {
    let netUnitCost: number;
    let taxAmount: number;
    let totalPrice: number;
    let productUnitDiscount: number = 0;
    let totalDiscountAmount: number = 0;

    if (purchaseReturnDetail.discountType === DiscountType.Fixed) {
      productUnitDiscount = purchaseReturnDetail.productUnitDiscount;
    } else if (purchaseReturnDetail.discountType === DiscountType.Percentage) {
      productUnitDiscount = (purchaseReturnDetail.productUnitPrice * purchaseReturnDetail.discountRate) / 100;
    }

    totalDiscountAmount = parseFloat((productUnitDiscount * purchaseReturnDetail.returnedQuantity).toFixed(2));
    const taxRateDecimal = purchaseReturnDetail.taxRate / 100;

    if (purchaseReturnDetail.taxMethod === TaxMethod.Exclusive) {
      netUnitCost = purchaseReturnDetail.productUnitCost - (productUnitDiscount || 0);
      const taxableTotalPrice = netUnitCost * purchaseReturnDetail.returnedQuantity;
      taxAmount = taxableTotalPrice * taxRateDecimal;
      totalPrice = taxableTotalPrice + taxAmount;
    }
    else if (purchaseReturnDetail.taxMethod === TaxMethod.Inclusive) {
      const priceAfterDiscount = purchaseReturnDetail.productUnitCost - (productUnitDiscount || 0);
      const taxRateFactor = 1 + taxRateDecimal;
      netUnitCost = priceAfterDiscount / taxRateFactor;
      taxAmount = (netUnitCost * purchaseReturnDetail.returnedQuantity) * (purchaseReturnDetail.taxRate / 100);
      totalPrice = (netUnitCost * purchaseReturnDetail.returnedQuantity) + taxAmount;
    }

    this.purchaseReturnDetails.at(index).patchValue({
      productUnitDiscount: parseFloat(productUnitDiscount.toFixed(2)),
      netUnitCost: parseFloat(netUnitCost.toFixed(2)),
      discountAmount: parseFloat(totalDiscountAmount.toFixed(2)),
      taxAmount: parseFloat(taxAmount.toFixed(2)),
      totalPrice: parseFloat(totalPrice.toFixed(2))
    }, { emitEvent: false });

    this.calculateFooterSection();
    this.calculateGrandTotal();
  }

  updateOrderDetail(index: number) {
    console.log(index)

    const purchaseReturnDetailFormGroup = this.purchaseReturnDetails.at(index) as FormGroup;
    const purchaseReturnDetail = purchaseReturnDetailFormGroup.value;
    this.customDialogService.open<{ purchaseReturnDetail: PurchaseReturnDetailModel; optionsDataSources: any }>(
      UpdatePurchaseOrderDetailComponent,
      { purchaseReturnDetail: purchaseReturnDetail, optionsDataSources: this.optionsDataSources },
      purchaseReturnDetail.productName
    )
      .subscribe((succeeded) => {
        if (succeeded) {

          if (this.closeDialogsubscription) {
            this.closeDialogsubscription.unsubscribe();
          }

          this.closeDialogsubscription = this.customDialogService.closeDataSubject.subscribe((updatedPurchaseReturnDetail: PurchaseReturnDetailModel) => {

            purchaseReturnDetailFormGroup.patchValue({
              productUnitCost: updatedPurchaseReturnDetail.productUnitCost,
              productUnitId: updatedPurchaseReturnDetail.productUnitId,
              taxMethod: updatedPurchaseReturnDetail.taxMethod,
              taxRate: updatedPurchaseReturnDetail.taxRate,
              discountType: updatedPurchaseReturnDetail.discountType,
              discountRate: updatedPurchaseReturnDetail.discountRate,
              productUnitDiscount: updatedPurchaseReturnDetail.productUnitDiscount,
            }, { emitEvent: false });
            const updatePurchaseReturnDetailValue = purchaseReturnDetailFormGroup.value;
            this.calculateTaxAndTotalPrice(index, updatePurchaseReturnDetailValue);

          });
        }
      });

  }

  private openDialog(index: number, purchaseReturnDetail: PurchaseReturnDetailModel) {
    this.customDialogService.open<{ purchaseReturnDetail: PurchaseReturnDetailModel; optionsDataSources: any }>(
      UpdatePurchaseOrderDetailComponent,
      { purchaseReturnDetail: purchaseReturnDetail, optionsDataSources: this.optionsDataSources },
      purchaseReturnDetail.productName
    )
      .subscribe((succeeded) => {
        if (succeeded) {
          this.closeDialogsubscription = this.customDialogService.closeDataSubject.subscribe((updatedPurchaseReturnDetail: PurchaseReturnDetailModel) => {
            const purchaseReturnDetailFormGroup = this.purchaseReturnDetails.at(index) as FormGroup;
            console.log(index)
            console.log(updatedPurchaseReturnDetail)
            console.log(purchaseReturnDetailFormGroup)

            purchaseReturnDetailFormGroup.patchValue({
              productUnitCost: updatedPurchaseReturnDetail.productUnitCost,
              productUnitId: updatedPurchaseReturnDetail.productUnitId,
              taxMethod: updatedPurchaseReturnDetail.taxMethod,
              taxRate: updatedPurchaseReturnDetail.taxRate,
              discountType: updatedPurchaseReturnDetail.discountType,
              discountRate: updatedPurchaseReturnDetail.discountRate,
              productUnitDiscount: updatedPurchaseReturnDetail.productUnitDiscount,
            }, { emitEvent: false });
            const updatePurchaseReturnDetailValue = purchaseReturnDetailFormGroup.value;
            this.calculateTaxAndTotalPrice(index, updatePurchaseReturnDetailValue);

          });
        }
      });
  }


  // #endregion

  // #region PurchaseReturnOrder Footer

  calculateFooterSection() {
    this.totalQuantity = this.purchaseReturnDetails.controls.reduce((acc, curr) => acc + (curr.get('returnedQuantity').value || 0), 0);
    this.totalDiscount = parseFloat(this.purchaseReturnDetails.controls.reduce((acc, curr) => acc + (curr.get('discountAmount').value || 0), 0).toFixed(2));
    this.totalTaxAmount = parseFloat(this.purchaseReturnDetails.controls.reduce((acc, curr) => acc + (curr.get('taxAmount').value || 0), 0).toFixed(2));
    this.subTotal = parseFloat(this.purchaseReturnDetails.controls.reduce((acc, curr) => acc + (curr.get('totalPrice').value || 0), 0).toFixed(2));
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

    const totalProducts = this.purchaseReturnDetails.length;
    this.totalItems = totalProducts > 0 ? `${this.purchaseReturnDetails.length}(${this.totalQuantity})` : '0';

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
