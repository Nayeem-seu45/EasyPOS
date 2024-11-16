import { Component, OnInit } from '@angular/core';
import { DiscountType, ProductSelectListModel, SaleReturnDetailModel, SaleReturnModel, SaleReturnsClient, TaxMethod, UpsertSaleReturnModel } from 'src/app/modules/generated-clients/api-service';
import { ActivatedRoute } from '@angular/router';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonConstants } from 'src/app/core/contants/common';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { DatePipe } from '@angular/common';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
// import { UpdateSaleReturnDetailComponent } from '../update-sale-return-detail/update-sale-return-detail.component';
import { Subscription } from 'rxjs';
import { UpdateSaleDetailComponent } from '../update-sale-detail/update-sale-detail.component';

@Component({
  selector: 'app-sale-return-detail',
  templateUrl: './sale-return-detail.component.html',
  styleUrl: './sale-return-detail.component.scss',
  providers: [SaleReturnsClient, DatePipe]
})
export class SaleReturnDetailComponent implements OnInit {

  id: string;
  saleId: string;
  processFrom: string;
  item: UpsertSaleReturnModel;
  DiscountType = DiscountType;
  optionsDataSources: any;
  saleReturnDate: string | null = null;
  CommonConstant = CommonConstants;

  discountTypes: { id: number, name: string }[] = [];
  selectedProduct: ProductSelectListModel | null = null;

  // Table footer section
  totalQuantity: number = 0;
  totalDiscount: number = 0;
  totalTaxAmount: number = 0;
  subTotal: number = 0;

  // Grand total Section
  totalItems: string = '0';

  private closeDialogsubscription: Subscription;

  get isEdit(): boolean{
    return this.id && this.id !== CommonConstants.EmptyGuid;
  }

  constructor(private entityClient: SaleReturnsClient,
    private activatedRoute: ActivatedRoute,
    private toast: ToastService,
    private datePipe: DatePipe,
    private customDialogService: CustomDialogService
  ) {
  }

  ngOnInit(): void {

    this.processFrom = this.activatedRoute.snapshot.data["processFrom"];

    this.activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id')
      this.saleId = params.get('saleId')
    });

    this.discountTypes = CommonUtils.enumToArray(DiscountType);

    // this.item = new SaleReturnModel({
    //   referenceNo: null,
    //   returnDate: null,
    //   warehouseId: null,
    //   customerId: null,
    //   billerId: null,
    //   returnStatusId: null,
    //   paymentStatusId: null,
    //   taxRate: null,
    //   taxAmount: null,
    //   discountType: DiscountType.Fixed,
    //   discountAmount: null,
    //   discountRate: null,
    //   shippingCost: 0,
    //   grandTotal: null,
    //   returnNote: null,
    //   staffNote: null,
    //   saleReturnDetails: [],
    // });
    console.log(this.processFrom)
    if (this.processFrom === 'return-list') {
      this.getById(this.id || CommonConstants.EmptyGuid)
    } else {
      this.getBySaleId(this.saleId || CommonConstants.EmptyGuid)
    }
  }

  ngOnDestroy() {
    // Unsubscribe when the component is destroyed to avoid memory leaks
    if (this.closeDialogsubscription) {
      this.closeDialogsubscription.unsubscribe();
    }
  }

  // #region CRUDS

  onSubmit() {
    if (!this.id || this.id === this.CommonConstant.EmptyGuid) {
      this.save();
    } else {
      this.update();
    }
  }

  getById(id: string) {
    this.entityClient.get(id).subscribe({
      next: (res: any) => {
        this.item = res;
        this.optionsDataSources = res.optionsDataSources;
        this.calculateGrandTotal();
      },
      error: (error) => {
        this.toast.showError(error);
      }
    });
  }

  getBySaleId(saleId: string) {
    this.entityClient.getBySaleId(saleId).subscribe({
      next: (res: any) => {
        this.item = res;
        this.optionsDataSources = res.optionsDataSources;
        this.calculateGrandTotal();
      },
      error: (error) => {
        this.toast.showError(error);
      }
    });
  }


  save(): void {
    this.entityClient.create(this.item).subscribe({
      next: () => {
        this.toast.created();
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  update(): void {
    this.entityClient.update(this.item).subscribe({
      next: () => {
        this.toast.updated();
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  // #endregion

  // #region Add or Update SaleReturnDetail

  onProductSelect() {
    if (this.selectedProduct) {
      this.addProductToSaleReturnDetails(this.selectedProduct);
      this.selectedProduct = null;
    }
  }

  onRemoveDetail(index: number, id: string) {
    this.item.saleReturnDetails.splice(index, 1);
    this.deleteSaleReturnDetailDelete(id);
    this.calculateGrandTotal();
  }

  private addProductToSaleReturnDetails(product: ProductSelectListModel) {
    const quantity = 1; // Default quantity
    // const totalDiscountAmount = (product.discountAmount || 0) * quantity;

    // Prepare the saleReturn detail model with computed values
    const productDetail = new SaleReturnDetailModel({
      productId: product.id,
      productCode: product.code,
      productName: product.name,
      productUnitCost: product.costPrice,
      productUnitPrice: product.salePrice,
      productUnitId: product.saleUnit,
      productUnitDiscount: product.discountAmount || 0,
      soldQuantity: quantity,
      returnedQuantity: quantity,
      discountType: product.discountType || DiscountType.Fixed,
      discountRate: product.discountAmount || 0,
      // discountAmount: parseFloat(totalDiscountAmount.toFixed(2)),
      taxRate: product.taxRate || 0,
      taxMethod: product.taxMethod,
      remarks: ''
    });

    // Calculate tax and total price
    this.calculateTaxAndTotalPrice(productDetail);

    // Push the saleReturn detail to the saleReturnDetails array
    this.item.saleReturnDetails.push(productDetail);

    this.calculateGrandTotal();
  }

  onItemPropsChange(productDetail: SaleReturnDetailModel) {
    // const quantity = productDetail.quantity;
    // const totalDiscountAmount = (productDetail.productUnitDiscount || 0) * quantity;
    // productDetail.discountAmount = parseFloat(totalDiscountAmount.toFixed(2));

    // Calculate tax and total price
    this.calculateTaxAndTotalPrice(productDetail);

    this.calculateGrandTotal();
  }

  private calculateTaxAndTotalPrice(productDetail: SaleReturnDetailModel) {
    let netUnitPrice: number;
    let taxAmount: number;
    let totalPrice: number;
    let productUnitDiscount: number = 0;
    // let totalDiscountAmount: number = 0;

    if (productDetail.discountType === DiscountType.Fixed) {
      productUnitDiscount = productDetail.productUnitDiscount;
    } else if (productDetail.discountType === DiscountType.Percentage) {
      productUnitDiscount = (productDetail.productUnitPrice * productDetail.discountRate) / 100;
    }

    const taxRateDecimal = productDetail.taxRate / 100;
    if (productDetail.taxMethod === TaxMethod.Exclusive) {
      // Exclusive tax method
      netUnitPrice = productDetail.productUnitPrice - (productUnitDiscount || 0);
      const taxableTotalPrice = netUnitPrice * productDetail.returnedQuantity;
      taxAmount = taxableTotalPrice * taxRateDecimal;
      totalPrice = taxableTotalPrice + taxAmount;
    } else if (productDetail.taxMethod === TaxMethod.Inclusive) {
      // Inclusive tax method
      const priceAfterDiscount = productDetail.productUnitPrice - (productUnitDiscount || 0);
      const taxRateFactor = 1 + taxRateDecimal;
      netUnitPrice = priceAfterDiscount / taxRateFactor;
      taxAmount = (netUnitPrice * productDetail.returnedQuantity) * taxRateDecimal;
      totalPrice = (netUnitPrice * productDetail.returnedQuantity) + taxAmount;
    }

    productDetail.netUnitPrice = parseFloat(netUnitPrice.toFixed(2));
    productDetail.productUnitDiscount = parseFloat(productUnitDiscount.toFixed(2));
    productDetail.discountAmount = parseFloat((productDetail.productUnitDiscount * productDetail.returnedQuantity).toFixed(2));
    productDetail.taxAmount = parseFloat(taxAmount.toFixed(2));
    productDetail.totalPrice = parseFloat(totalPrice.toFixed(2));
  }

  private deleteSaleReturnDetailDelete(id: string) {
    if (!id || id === CommonConstants.EmptyGuid) {
      return;
    }

    this.entityClient.deleteSaleReturnDetail(id).subscribe({
      next: () => {
        console.log('delete detail')
      }, error: (error) => {
        console.log(error)
      }
    });
  }

  updateDetail(index: number, saleReturnDetail: SaleReturnDetailModel) {
    this.openDialog(index, saleReturnDetail);
  }

  private openDialog(index: number, saleReturnDetail: SaleReturnDetailModel) {
    this.customDialogService.open<{ saleReturnDetail: SaleReturnDetailModel; optionsDataSources: any }>(
      UpdateSaleDetailComponent,
      { saleReturnDetail: saleReturnDetail, optionsDataSources: this.optionsDataSources },
      saleReturnDetail.productName
    )
      .subscribe((succeeded) => {
        if (succeeded) {

          if (this.closeDialogsubscription) {
            this.closeDialogsubscription.unsubscribe();
          }

          this.closeDialogsubscription = this.customDialogService.closeDataSubject.subscribe((updateSaleReturnDetail: SaleReturnDetailModel) => {
            this.item.saleReturnDetails[index].productUnitPrice = updateSaleReturnDetail.productUnitPrice;
            this.item.saleReturnDetails[index].productUnitId = updateSaleReturnDetail.productUnitId;
            this.item.saleReturnDetails[index].taxMethod = updateSaleReturnDetail.taxMethod;
            this.item.saleReturnDetails[index].taxRate = updateSaleReturnDetail.taxRate;
            this.item.saleReturnDetails[index].discountType = updateSaleReturnDetail.discountType;
            this.item.saleReturnDetails[index].discountRate = updateSaleReturnDetail.discountRate;
            this.item.saleReturnDetails[index].productUnitDiscount = updateSaleReturnDetail.productUnitDiscount;
            this.calculateTaxAndTotalPrice(this.item.saleReturnDetails[index])
          });
        }
      });
  }


  // #endregion

  // #region SaleReturnsOrder Footer 

  calculateFooterSection() {
    this.totalQuantity = this.item.saleReturnDetails.reduce((total, detail) => total + detail.returnedQuantity, 0);
    this.totalDiscount = this.item.saleReturnDetails.reduce((total, detail) => total + (detail.discountAmount || 0), 0);
    this.totalTaxAmount = this.item.saleReturnDetails.reduce((total, detail) => total + (detail.taxAmount || 0), 0);
    const subTotal = this.item.saleReturnDetails.reduce((total, saleReturnDetail) => {
      return total + saleReturnDetail.totalPrice;
    }, 0) || 0;
    this.item.subTotal = subTotal;
  }

  getTotalQuantity(): number {
    return this.item.saleReturnDetails.reduce((total, detail) => total + detail.returnedQuantity, 0);
  }

  getTotalDiscount(): number {
    const totalDiscount = this.item.saleReturnDetails.reduce((total, detail) => total + (detail.discountAmount || 0), 0);
    return parseFloat(totalDiscount.toFixed(2));
  }

  getTotalTax(): number {
    const totalTaxAmount = this.item.saleReturnDetails.reduce((total, detail) => total + (detail.taxAmount || 0), 0);
    return parseFloat(totalTaxAmount.toFixed(2));
  }

  // Function to calculate the grand total
  getSubTotalOfTotal(): number {
    const subTotal = this.item.saleReturnDetails.reduce((total, saleReturnDetail) => {
      return total + saleReturnDetail.totalPrice;
    }, 0) || 0;
    this.item.subTotal = subTotal;
    return parseFloat(subTotal.toFixed(2));
  }

  // #endregion

  // #region Grand Total Section

  onOrderTaxChange() {
    this.calculateGrandTotal();
  }

  onDiscountTypeChange() {
    this.item.discountRate = null;
    this.item.discountAmount = 0;

    this.calculateGrandTotal();
  }

  onDiscountRateChange() {
    this.calculateGrandTotal();
  }

  onDiscountAmountChange() {
    if (!this.item.discountAmount) {
      this.item.discountAmount = 0;
    }
    this.calculateGrandTotal();
  }

  onShippingCostChange() {
    if (!this.item.shippingCost) {
      this.item.shippingCost = 0;
    }
    this.calculateGrandTotal();
  }

  private calculateGrandTotal() {
    const subTotalOfTotal = this.getSubTotalOfTotal();
    if (this.item.discountType === DiscountType.Percentage) {
      const discountAmount = subTotalOfTotal * (this.item.discountRate / 100)
      this.item.discountAmount = parseFloat(discountAmount.toFixed(2)) || 0;
    }

    const taxableAmount = subTotalOfTotal - this.item.discountAmount;
    this.item.taxAmount = parseFloat(((taxableAmount * this.item.taxRate) / 100).toFixed(2));

    // Calculate grand total
    this.item.grandTotal = parseFloat(
      (subTotalOfTotal + this.item.taxAmount + (this.item.shippingCost || 0) - this.item.discountAmount).toFixed(2)
    );
    const totalProducts = this.item.saleReturnDetails.length;
    this.totalItems = totalProducts > 0 ? `${this.item.saleReturnDetails.length}(${this.getTotalQuantity()})` : '0';
  }

  // #endregion

  // #region Other
  onFileUpload(fileUrl) {

  }


  // #endregion

}
