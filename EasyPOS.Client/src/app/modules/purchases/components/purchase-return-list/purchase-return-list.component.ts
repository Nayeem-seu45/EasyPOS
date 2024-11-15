import { Component, inject, ViewChild } from '@angular/core';
import { PurchaseReturnInfoModel, PurchaseReturnModel, PurchaseReturnsClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { DataGridComponent } from 'src/app/shared/components/data-grid/data-grid.component';
import { CommonConstants } from 'src/app/core/contants/common';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';
import { DatePipe } from '@angular/common';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { PurchaseReturnDetailComponent } from '../purchase-return-detail/purchase-return-detail.component';

@Component({
  selector: 'app-purchase-return-list',
  templateUrl: './purchase-return-list.component.html',
  styleUrl: './purchase-return-list.component.scss',
  providers: [PurchaseReturnsClient, DatePipe]
})
export class PurchaseReturnListComponent {
  detailComponent = PurchaseReturnDetailComponent;
  pageId = '90019f10-65ff-4baf-2b4a-08dd001bc922'

  item: PurchaseReturnInfoModel;

  @ViewChild('grid') grid: DataGridComponent;
  entityClient: PurchaseReturnsClient = inject(PurchaseReturnsClient);
  constructor(private customDialogService: CustomDialogService,
    private datePipe: DatePipe
  ) {

  }

  onhandleGridRowAction(event) {
  if (event.action.actionName === 'pdf') {
      this.generatePurchaseReturnDetailPdf(event);
    }
  }

  private async generatePurchaseReturnDetailPdf(event: any) {
    await this.getDetailById(event.data.id);
    this.exportPdf();
  }

  private exportPdf() {
    const doc = new jsPDF('p', 'mm', 'a4'); // Create jsPDF instance

    // Header: PurchaseReturn Details
    doc.setFontSize(16);
    doc.setTextColor(40);
    doc.text('PurchaseReturn Details', 10, 15);

    // Section: Company and Supplier Info
    const companyText = `From:\n${this.item.companyInfo.name}\n${this.item.companyInfo.address}, ${this.item.companyInfo.city}, ${this.item.companyInfo.country}\nPhone: ${this.item.companyInfo.phone}\nMobile: ${this.item.companyInfo.mobile}\nEmail: ${this.item.companyInfo.email}`;
    const supplierText = `Supplier:\n${this.item.supplier.name}\n${this.item.supplier.address}, ${this.item.supplier.city}, ${this.item.supplier.country}\nPhone: ${this.item.supplier.phoneNo}\nMobile: ${this.item.supplier.mobile}\nEmail: ${this.item.supplier.email}`;
    const referenceText = `Reference: ${this.item.referenceNo}\nPurchaseReturn Status: ${this.item.purchaseReturnStatus}\nPayment Status: ${this.item.paymentStatusId}`;

    // Display company, supplier, and reference info
    doc.setFontSize(10);
    doc.setTextColor(0);

    // Calculate positions based on equal division and margins
    const leftMargin = 10;
    const sectionWidth = (doc.internal.pageSize.getWidth() - leftMargin * 2) / 3; // 3 equal sections

    // Positioning the texts
    doc.text(companyText, leftMargin, 30);                     // Company Info
    doc.text(supplierText, leftMargin + sectionWidth, 30);    // Supplier Info
    doc.text(referenceText, leftMargin + sectionWidth * 2, 30); // Reference Info


    // Section: Items Table
    const itemHeaders = ['#', 'Name', 'Price', 'Quantity', 'Unit Price', 'Discount', 'Tax', 'Sub Total'];
    const itemBody = this.item.purchaseReturnDetails.map((detail, index) => [
      index + 1,
      `${detail.productName} (${detail.productCode})`,
      detail.productUnitCost,
      detail.returnedQuantity,
      detail.netUnitCost.toFixed(2),
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
            doc.save('purchaseReturn-details.pdf');
          }
        });
      },
      margin: { top: 70, bottom: 10, left: leftMargin, right: rightMargin }, // Set margins for the table
    });
  }

  private exportPdf2() {
    const doc = new jsPDF('p', 'mm', 'a4'); // Create jsPDF instance

    // Header: PurchaseReturn Details
    doc.setFontSize(16);
    doc.setTextColor(40);
    doc.text('PurchaseReturn Details', 10, 15);

    // Section: Company and Supplier Info
    const companyText = `From:\n${this.item.companyInfo.name}\n${this.item.companyInfo.address}, ${this.item.companyInfo.city}, ${this.item.companyInfo.country}\nPhone: ${this.item.companyInfo.phone}\nMobile: ${this.item.companyInfo.mobile}\nEmail: ${this.item.companyInfo.email}`;
    const supplierText = `Supplier:\n${this.item.supplier.name}\n${this.item.supplier.address}, ${this.item.supplier.city}, ${this.item.supplier.country}\nPhone: ${this.item.supplier.phoneNo}\nMobile: ${this.item.supplier.mobile}\nEmail: ${this.item.supplier.email}`;
    const referenceText = `Reference: ${this.item.referenceNo}\nPurchaseReturn Status: ${this.item.purchaseReturnStatus}\nPayment Status: ${this.item.paymentStatusId}`;

    // Display company, supplier, and reference info
    doc.setFontSize(10);
    doc.setTextColor(0);

    // Calculate positions based on equal division and margins
    const leftMargin = 10;
    const sectionWidth = (doc.internal.pageSize.getWidth() - leftMargin * 2) / 3; // 3 equal sections

    // Positioning the texts
    doc.text(companyText, leftMargin, 30);                     // Company Info
    doc.text(supplierText, leftMargin + sectionWidth, 30);    // Supplier Info
    doc.text(referenceText, leftMargin + sectionWidth * 2, 30); // Reference Info


    // Section: Items Table
    const itemHeaders = ['#', 'Name', 'Price', 'Quantity', 'Unit Price', 'Discount', 'Tax', 'Sub Total'];
    const itemBody = this.item.purchaseReturnDetails.map((detail, index) => [
      index + 1,
      `${detail.productName} (${detail.productCode})`,
      detail.productUnitCost,
      detail.returnedQuantity,
      detail.netUnitCost.toFixed(2),
      detail.discountAmount.toFixed(2),
      detail.taxAmount.toFixed(2),
      detail.totalPrice.toFixed(2)
    ]);

    // Generate Items Table
    autoTable(doc, {
      head: [itemHeaders],
      body: itemBody,
      startY: 70,
      styles: { fontSize: 9 },
      didDrawPage: (data) => {
        // Move to after the table to continue text
        const finalYAfterItemsTable = data.cursor.y + 10; // Get the Y position after items table

        // Section: Payment Table
        // const paymentHeaders = ['#', 'Date', 'Payment Type', 'Note', 'Amount'];
        // const paymentBody = this.item.paymentDetails.map((payment, index) => [
        //   index + 1,
        //   this.datePipe.transform(payment.paymentDate, 'dd/MM/yyyy'),
        //   payment.paymentTypeName,
        //   payment.note,
        //   payment.payingAmount.toFixed(2)
        // ]);

        // Generate Payment Table using autoTable
        autoTable(doc, {
          // head: [paymentHeaders],
          // body: paymentBody,
          startY: finalYAfterItemsTable, // Adjust startY to be below the items table
          styles: { fontSize: 9 },
          didDrawPage: (paymentData) => {
            const finalYAfterPaymentsTable = paymentData.cursor.y + 10; // Get the Y position after payments table

            // Section: Footer Summary
            doc.setFontSize(10);
            doc.text(`Items: ${this.item.totalItems}`, 10, finalYAfterPaymentsTable + 10);
            doc.text(`Total: ${this.item.subTotal.toFixed(2)}`, 10, finalYAfterPaymentsTable + 20);
            doc.text(`Order Tax: ${this.item.taxAmount.toFixed(2)}`, 10, finalYAfterPaymentsTable + 30);
            doc.text(`Order Discount: ${this.item.discountAmount.toFixed(2)}`, 10, finalYAfterPaymentsTable + 40);
            doc.text(`Shipping Cost: ${this.item.shippingCost.toFixed(2)}`, 10, finalYAfterPaymentsTable + 50);
            doc.text(`Grand Total: ${this.item.grandTotal.toFixed(2)}`, 10, finalYAfterPaymentsTable + 60);

            // Save the PDF
            doc.save('purchaseReturn-details.pdf');
          }
        });
      }
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
