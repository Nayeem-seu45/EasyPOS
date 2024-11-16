using EasyPOS.Domain.Accounting;
using EasyPOS.Domain.Admin;
using EasyPOS.Domain.Common;
using EasyPOS.Domain.HRM;
using EasyPOS.Domain.Products;
using EasyPOS.Domain.ProductTransfers;
using EasyPOS.Domain.Purchases;
using EasyPOS.Domain.Quotations;
using EasyPOS.Domain.Sales;
using EasyPOS.Domain.Settings;
using EasyPOS.Domain.Stakeholders;
using Unit = EasyPOS.Domain.Products.Unit;

namespace EasyPOS.Application.Common.Abstractions;

public interface IApplicationDbContext
{
    //DbSet<RefreshToken> RefreshTokens { get; }

    #region Admin

    DbSet<AppMenu> AppMenus { get; }
    DbSet<RoleMenu> RoleMenus { get; }
    DbSet<AppPage> AppPages { get; }
    DbSet<AppNotification> AppNotifications { get; }
    #endregion

    #region Common Setup
    DbSet<Lookup> Lookups { get; }

    DbSet<LookupDetail> LookupDetails { get; }

    #endregion

    #region Products

    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Unit> Units { get; }
    DbSet<Tax> Taxes { get; }
    DbSet<ProductAdjustment> ProductAdjustments { get; }
    DbSet<ProductAdjustmentDetail> ProductAdjustmentDetails { get; }
    DbSet<CountStock> CountStocks { get; }
    DbSet<CountStockCategory> CountStockCategories { get; }
    DbSet<CountStockBrand> CountStockBrands { get; }

    #endregion

    #region Product Transfer
    DbSet<ProductTransfer> ProductTransfers { get; }
    DbSet<ProductTransferDetail> ProductTransferDetails { get; }
    #endregion

    #region Stakeholders
    DbSet<Customer> Customers { get; }
    DbSet<CustomerGroup> CustomerGroups { get; }
    DbSet<Supplier> Suppliers { get; }

    #endregion

    #region Purchase
    DbSet<Purchase> Purchases { get; }
    DbSet<PurchaseDetail> PurchaseDetails { get; }
    DbSet<PurchasePayment> PurchasePayments { get; }
    DbSet<PurchaseReturn> PurchaseReturns { get; }
    DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; }
    DbSet<PurchaseReturnPayment> PurchaseReturnPayments { get; }

    #endregion

    #region Sales

    DbSet<Sale> Sales { get; }
    DbSet<SaleDetail> SaleDetails { get; }
    DbSet<SaleReturn> SaleReturns { get; }
    DbSet<SaleReturnDetail> SaleReturnDetails { get; }
    DbSet<SalePayment> SalePayments { get; }
    DbSet<SaleReturnPayment> SaleReturnPayments { get; }
    DbSet<Courier> Couriers { get; }
    DbSet<Coupon> Coupons { get; }
    DbSet<GiftCard> GiftCards { get; }


    #endregion


    #region Quotations
    DbSet<Quotation> Quotations { get; }
    DbSet<QuotationDetail> QuotationDetails { get; }
    #endregion

    #region Accounting
    DbSet<Account> Accounts { get; }
    DbSet<MoneyTransfer> MoneyTransfers { get; }
    DbSet<Expense> Expenses { get; }

    #endregion

    #region HRM
    DbSet<Department> Departments { get; }
    DbSet<Designation> Designations { get; }
    DbSet<Employee> Employees { get; }
    DbSet<WorkingShift> WorkingShifts { get; }
    DbSet<WorkingShiftDetail> WorkingShiftDetails { get; }
    DbSet<Holiday> Holidays { get; }
    DbSet<Attendance> Attendances { get; }
    DbSet<LeaveRequest> LeaveRequests { get; }
    DbSet<LeaveType> LeaveTypes { get; }

    #endregion

    #region Settings
    DbSet<CompanyInfo> CompanyInfos { get; }
    #endregion

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
