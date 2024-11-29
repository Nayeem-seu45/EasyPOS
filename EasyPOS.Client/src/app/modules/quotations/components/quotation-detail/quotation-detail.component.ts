import { Component, OnInit } from '@angular/core';
import { DiscountType, ProductSelectListModel, QuotationDetailModel, QuotationModel, QuotationsClient, TaxMethod, UpsertQuotationModel } from 'src/app/modules/generated-clients/api-service';
import { ActivatedRoute } from '@angular/router';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonConstants } from 'src/app/core/contants/common';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { DatePipe } from '@angular/common';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { Subscription } from 'rxjs';
import { UpdateQuotationDetailComponent } from '../update-quotation-detail/update-quotation-detail.component';

@Component({
  selector: 'app-quotation-detail',
  templateUrl: './quotation-detail.component.html',
  styleUrl: './quotation-detail.component.scss',
  providers: [QuotationsClient, DatePipe]
})
export class QuotationDetailComponent implements OnInit {

  id: string;
  item: UpsertQuotationModel;
  DiscountType = DiscountType;
  optionsDataSources: any;
  quotationDate: string | null = null;
  CommonConstant = CommonConstants;

  discountTypes: { id: number, name: string }[] = [];
  // quotationStatusSelectList: { id: number, name: string }[] = [];
  selectedProduct: ProductSelectListModel | null = null;

  // Table footer section
  totalQuantity: number = 0;
  totalDiscount: number = 0;
  totalTaxAmount: number = 0;
  subTotal: number = 0;

  // Grand total Section
  totalItems: string = '0';

  private closeDialogsubscription: Subscription;


  constructor(private entityClient: QuotationsClient,
    private activatedRoute: ActivatedRoute,
    private toast: ToastService,
    private datePipe: DatePipe,
    private customDialogService: CustomDialogService
    ) {
  }

  ngOnInit(): void {

    this.activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id')
    });

    this.discountTypes = CommonUtils.enumToArray(DiscountType);

    this.item = new QuotationModel({
      referenceNo: null,
      quotationDate: null,
      warehouseId: null,
      customerId: null,
      billerId: null,
      quotationStatusId: null,
      taxRate: null,
      taxAmount: null,
      discountType: DiscountType.Fixed,
      discountAmount: null,
      discountRate: null,
      shippingCost: 0,
      grandTotal: null,
      quotationNote: null,
      quotationDetails: [],
    });

    this.getById(this.id || this.CommonConstant.EmptyGuid)
  }

  ngOnDestroy() {
    // Unsubscribe when the component is destroyed to avoid memory leaks
    if (this.closeDialogsubscription) {
      this.closeDialogsubscription.unsubscribe();
    }
  }

  // #region CRUDS

  onFormSubmit() {
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

  // #region Add or Update QuotationDetail

  onProductSelect() {
    if (this.selectedProduct) {
      this.addProductToQuotationDetails(this.selectedProduct);
      this.selectedProduct = null;
    }
  }

  onRemoveQuotationDetail(index: number, id: string) {
    this.item.quotationDetails.splice(index, 1);
    this.deleteQuotationDetailDelete(id);
    this.calculateGrandTotal();
  }

  private addProductToQuotationDetails(product: ProductSelectListModel) {
    const quantity = 1; // Default quantity
    // const totalDiscountAmount = (product.discountAmount || 0) * quantity;

    // Prepare the quotation detail model with computed values
    const productDetail = new QuotationDetailModel({
      productId: product.id,
      productCode: product.code,
      productName: product.name,
      productUnitCost: product.costPrice,
      productUnitPrice: product.salePrice,
      productUnitId: product.saleUnit,
      productUnitDiscount: product.discountAmount || 0,
      quantity: quantity,
      discountType: product.discountType || DiscountType.Fixed,
      discountRate: product.discountAmount || 0,
      // discountAmount: parseFloat(totalDiscountAmount.toFixed(2)),
      taxRate: product.taxRate || 0,
      taxMethod: product.taxMethod,
      remarks: ''
    });

    // Calculate tax and total price
    this.calculateTaxAndTotalPrice(productDetail);

    // Push the quotation detail to the quotationDetails array
    this.item.quotationDetails.push(productDetail);

    this.calculateGrandTotal();
  }

  onItemPropsChange(productDetail: QuotationDetailModel) {
    this.calculateTaxAndTotalPrice(productDetail);

    this.calculateGrandTotal();
  }

  private calculateTaxAndTotalPrice(productDetail: QuotationDetailModel) {
    let netUnitPrice: number;
    let taxAmount: number;
    let totalPrice: number;
    let productUnitDiscount: number = 0;
    // let totalDiscountAmount: number = 0;

    if(productDetail.discountType === DiscountType.Fixed){
      productUnitDiscount = productDetail.productUnitDiscount;
    } else if(productDetail.discountType === DiscountType.Percentage){
      productUnitDiscount = (productDetail.productUnitPrice * productDetail.discountRate) / 100;
    }
    
    const taxRateDecimal = productDetail.taxRate / 100;
    if (productDetail.taxMethod === TaxMethod.Exclusive) {
      // Exclusive tax method
      netUnitPrice = productDetail.productUnitPrice - (productUnitDiscount || 0);
      const taxableTotalPrice = netUnitPrice * productDetail.quantity;
      taxAmount = taxableTotalPrice * taxRateDecimal;
      totalPrice = taxableTotalPrice + taxAmount;
    } else if (productDetail.taxMethod === TaxMethod.Inclusive) {
      // Inclusive tax method
      const priceAfterDiscount = productDetail.productUnitPrice - (productUnitDiscount || 0);
      const taxRateFactor = 1 + taxRateDecimal;
      netUnitPrice = priceAfterDiscount / taxRateFactor;
      taxAmount = (netUnitPrice * productDetail.quantity) * taxRateDecimal;
      totalPrice = (netUnitPrice * productDetail.quantity) + taxAmount;
    }

    productDetail.netUnitPrice = parseFloat(netUnitPrice.toFixed(2));
    productDetail.productUnitDiscount = parseFloat(productUnitDiscount.toFixed(2));
    productDetail.discountAmount = parseFloat((productDetail.productUnitDiscount * productDetail.quantity).toFixed(2));
    productDetail.taxAmount = parseFloat(taxAmount.toFixed(2));
    productDetail.totalPrice = parseFloat(totalPrice.toFixed(2));
  }

  private deleteQuotationDetailDelete(id: string) {
    if (!id || id === CommonConstants.EmptyGuid) {
      return;
    }

    this.entityClient.deleteQuotationDetail(id).subscribe({
      next: () => {
        console.log('delete detail')
      }, error: (error) => {
        console.log(error)
      }
    });
  }

  updateQuotationDetail(index: number, quotationDetail: QuotationDetailModel){
    this.openDialog(index, quotationDetail);
  }

  private openDialog(index: number, quotationDetail: QuotationDetailModel) {
    this.customDialogService.open<{ quotationDetail: QuotationDetailModel; optionsDataSources: any }>(
      UpdateQuotationDetailComponent,
      { quotationDetail: quotationDetail, optionsDataSources: this.optionsDataSources },
      quotationDetail.productName
    )
      .subscribe((succeeded) => {
        if (succeeded) {

          if (this.closeDialogsubscription) {
            this.closeDialogsubscription.unsubscribe();
          }

          this.closeDialogsubscription = this.customDialogService.closeDataSubject.subscribe((updateQuotationDetail: QuotationDetailModel) => {
            this.item.quotationDetails[index].productUnitPrice = updateQuotationDetail.productUnitPrice;
            this.item.quotationDetails[index].productUnitId = updateQuotationDetail.productUnitId;
            this.item.quotationDetails[index].taxMethod = updateQuotationDetail.taxMethod;
            this.item.quotationDetails[index].taxRate = updateQuotationDetail.taxRate;
            this.item.quotationDetails[index].discountType = updateQuotationDetail.discountType;
            this.item.quotationDetails[index].discountRate = updateQuotationDetail.discountRate;
            this.item.quotationDetails[index].productUnitDiscount = updateQuotationDetail.productUnitDiscount;
            this.calculateTaxAndTotalPrice(this.item.quotationDetails[index])
          });
        } 
      });
  }
  

  // #endregion

  // #region QuotationsOrder Footer 

  calculateFooterSection() {
    this.totalQuantity = this.item.quotationDetails.reduce((total, detail) => total + detail.quantity, 0);
    this.totalDiscount = this.item.quotationDetails.reduce((total, detail) => total + (detail.discountAmount || 0), 0);
    this.totalTaxAmount = this.item.quotationDetails.reduce((total, detail) => total + (detail.taxAmount || 0), 0);
    const subTotal = this.item.quotationDetails.reduce((total, quotationDetail) => {
      return total + quotationDetail.totalPrice;
    }, 0) || 0;
    this.item.subTotal = subTotal;
  }

  getTotalQuantity(): number {
    return this.item.quotationDetails.reduce((total, detail) => total + detail.quantity, 0);
  }

  getTotalDiscount(): number {
    const totalDiscount = this.item.quotationDetails.reduce((total, detail) => total + (detail.discountAmount || 0), 0);
    return parseFloat(totalDiscount.toFixed(2));
  }

  getTotalTax(): number {
    const totalTaxAmount = this.item.quotationDetails.reduce((total, detail) => total + (detail.taxAmount || 0), 0);
    return parseFloat(totalTaxAmount.toFixed(2));
  }

  // Function to calculate the grand total
  getSubTotalOfTotal(): number {
    const subTotal = this.item.quotationDetails.reduce((total, quotationDetail) => {
      return total + quotationDetail.totalPrice;
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
    const totalProducts = this.item.quotationDetails.length;
    this.totalItems = totalProducts > 0 ? `${this.item.quotationDetails.length}(${this.getTotalQuantity()})` : '0';
  }

  // #endregion

  // #region Other
  onFileUpload(fileUrl) {

  }

  // #endregion

}
