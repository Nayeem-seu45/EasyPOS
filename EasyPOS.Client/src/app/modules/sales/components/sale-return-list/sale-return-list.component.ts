import { Component, inject, ViewChild } from '@angular/core';
import { SaleReturnDetailComponent } from '../sale-return-detail/sale-return-detail.component';
import { SaleReturnInfoModel, SaleReturnModel, SaleReturnsClient } from 'src/app/modules/generated-clients/api-service';
import { DatePipe } from '@angular/common';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { CommonConstants } from 'src/app/core/contants/common';
import { DataGridComponent } from 'src/app/shared/components/data-grid/data-grid.component';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { SalePaymentListComponent } from '../sale-payment-list/sale-payment-list.component';
import { SalePaymentDetailComponent } from '../sale-payment-detail/sale-payment-detail.component';
// import { SaleReturnPaymentDetailComponent } from '../sale-return-payment-detail/sale-return-payment-detail.component';
// import { SaleReturnPaymentListComponent } from '../sale-return-payment-list/sale-return-payment-list.component';

@Component({
  selector: 'app-sale-return-list',
  templateUrl: './sale-return-list.component.html',
  styleUrl: './sale-return-list.component.scss',
  providers: [SaleReturnsClient, DatePipe]
})
export class SaleReturnListComponent {
  detailComponent = SaleReturnDetailComponent;
  pageId = 'e6ad6749-bca1-43b0-00f2-08dd00caa1fb'

  item: SaleReturnInfoModel;

  @ViewChild('grid') grid: DataGridComponent;
  entityClient: SaleReturnsClient = inject(SaleReturnsClient);

  constructor(private customDialogService: CustomDialogService,
    private datePipe: DatePipe
  ) {

  }

  onhandleGridRowAction(event) {

    if (event.action.actionName === 'addPayment') {
      this.addPayment(event);
    } else if (event.action.actionName === 'paymentList') {
      this.openPaymentList(event);
    } else if (event.action.actionName === 'pdf') {
      this.generateSaleReturnDetailPdf(event);
    }
  }

  private openPaymentList(event: any) {
    console.log(event)
    this.customDialogService.handleCloseIcon = false;
    const paymentListDialogRef = this.customDialogService.openDialog<SaleReturnModel>(
      SalePaymentListComponent,
      event.data,
      'Payment List',
      { width: '60vw' },
      // null,
      true
    );
    paymentListDialogRef.onClose.subscribe((succeeded) => {
      this.grid.refreshGrid();
    });
  }

  private addPayment(event: any) {
    this.customDialogService.open<{ id: string; saleReturn: SaleReturnModel; }>(
      SalePaymentDetailComponent,
      { id: CommonConstants.EmptyGuid, saleReturn: event.data },
      'Add Payment').subscribe((succeeded) => {
        if (succeeded) {
          this.grid.refreshGrid();
        }
      });
  }

  private async generateSaleReturnDetailPdf(event: any) {

    await this.getDetailById(event.data.id);

    this.exportPdf();

  }

  private exportPdf() {
    const doc = new jsPDF('p', 'mm', 'a4'); // Create jsPDF instance

    // Header: SaleReturn Details
    doc.setFontSize(16);
    doc.setTextColor(40);
    doc.text('SaleReturn Details', 10, 15);

    // Section: Company and Supplier Info
    const companyText = `From:\n${this.item.companyInfo.name}\n${this.item.companyInfo.address}, ${this.item.companyInfo.city}, ${this.item.companyInfo.country}\nPhone: ${this.item.companyInfo.phone}\nMobile: ${this.item.companyInfo.mobile}\nEmail: ${this.item.companyInfo.email}`;
    const customerText = `Customer:\n${this.item.customer.name}\n${this.item.customer.address}, ${this.item.customer.city}, ${this.item.customer.country}\nPhone: ${this.item.customer.phoneNo}\nMobile: ${this.item.customer.mobile}\nEmail: ${this.item.customer.email}`;
    const referenceText = `Reference: ${this.item.referenceNo}\nSaleReturn Status: ${this.item.returnStatus}\nPayment Status: ${this.item.paymentStatusId}`;

    // Display company, customer, and reference info
    doc.setFontSize(10);
    doc.setTextColor(0);

    // Calculate positions based on equal division and margins
    const leftMargin = 10;
    const sectionWidth = (doc.internal.pageSize.getWidth() - leftMargin * 2) / 3; // 3 equal sections

    // Positioning the texts
    doc.text(companyText, leftMargin, 30);                     // Company Info
    doc.text(customerText, leftMargin + sectionWidth, 30);    // Supplier Info
    doc.text(referenceText, leftMargin + sectionWidth * 2, 30); // Reference Info


    // Section: Items Table
    const itemHeaders = ['#', 'Name', 'Price', 'Quantity', 'Unit Price', 'Discount', 'Tax', 'Sub Total'];
    const itemBody = this.item.saleReturnDetails.map((detail, index) => [
      index + 1,
      `${detail.productName} (${detail.productCode})`,
      detail.productUnitCost,
      detail.returnedQuantity,
      detail.netUnitPrice.toFixed(2),
      detail.discountAmount.toFixed(2),
      detail.taxAmount.toFixed(2),
      detail.totalPrice.toFixed(2)
    ]);

    // Footer row with totals
    const footerRow = [
      'Total',
      '',
      '',
      this.item.totalQuantity.toFixed(2), // Sum of quantities
      '',
      this.item.totalDiscount.toFixed(2), // Total Discount
      this.item.totalTaxAmount.toFixed(2),      // Total Tax
      this.item.subTotal.toFixed(2)  // Total Sub Total
    ];

    itemBody.push(footerRow); // Append footer row

    // Calculate available width for the table
    const rightMargin = 10;
    const tableWidth = doc.internal.pageSize.getWidth() - leftMargin - rightMargin; // Full width minus margins

    // Generate Items Table
    autoTable(doc, {
      head: [itemHeaders],
      body: itemBody,
      startY: 70,
      styles: { fontSize: 9 },
      columnStyles: {
        0: { cellWidth: 10 }, // You can adjust this to your liking
        1: { cellWidth: tableWidth * 0.25 }, // 25% of the total width for the Name column
        2: { cellWidth: tableWidth * 0.15 }, // 15% of the total width for Price column
        3: { cellWidth: tableWidth * 0.1 },  // 10% of the total width for Quantity column
        4: { cellWidth: tableWidth * 0.15 }, // 15% of the total width for Unit Price column
        5: { cellWidth: tableWidth * 0.1 },  // 10% of the total width for Discount column
        6: { cellWidth: tableWidth * 0.1 },  // 10% of the total width for Tax column
        7: { cellWidth: tableWidth * 0.1 },  // 10% of the total width for Sub Total column
      },
      didDrawPage: (data) => {
        const footerYPosition = data.cursor.y + 10; // Get the Y position after items table

        const footerTableWidth = (doc.internal.pageSize.getWidth() / 2) - rightMargin;
        const footerLeftMargin = (doc.internal.pageSize.getWidth() / 2);

        // Section: Footer Summary Data
        const footerData = [
          [`Items`, `${this.item.totalItems}`],
          [`Total`, `${this.item.subTotal.toFixed(2)}`],
          [`Order Tax`, `${this.item.taxAmount.toFixed(2)}`],
          [`Order Discount`, `${this.item.discountAmount.toFixed(2)}`],
          [`Shipping Cost`, `${this.item.shippingCost.toFixed(2)}`],
          [`Grand Total`, `${this.item.grandTotal.toFixed(2)}`],
        ];

        // Generate Footer Table
        autoTable(doc, {
          body: footerData,
          startY: footerYPosition,
          theme: 'grid', // Add borders
          styles: {
            fontSize: 10,
            cellPadding: 2,
            overflow: 'linebreak',
            lineColor: [0, 0, 0],
            fillColor: [255, 255, 255],
            textColor: [0, 0, 0],
            halign: 'right', // Right align text
          },
          columnStyles: {
            0: { cellWidth: footerTableWidth * 0.5, halign: 'right' }, // Description column
            1: { cellWidth: footerTableWidth * 0.5, halign: 'right' }, // Amount column
          },
          margin: { left: footerLeftMargin }, // Set left margin for the table
          didDrawPage: () => {
            // Save the PDF
            doc.save('saleReturn-details.pdf');
          }
        });
      },
      margin: { top: 70, bottom: 10, left: leftMargin, right: rightMargin }, // Set margins for the table
    });
  }

  // Fetches the item details and calculates the footer section
  private async getDetailById(id: string) {
    return new Promise<void>((resolve, reject) => {
      this.entityClient.getDetail(id).subscribe({
        next: (res: any) => {
          this.item = res;
          resolve(); // Resolves after item data is set and footer is calculated
        },
        error: (error) => {
          this.grid.toast.showError(CommonUtils.getErrorMessage(error));
          reject(error); // Reject in case of an error
        }
      });
    });
  }
}
