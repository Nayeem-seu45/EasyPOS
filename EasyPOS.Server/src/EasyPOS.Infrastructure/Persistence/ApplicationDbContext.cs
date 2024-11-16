using System.Reflection;
using EasyPOS.Application.Common.Abstractions;
using EasyPOS.Domain.Accounting;
using EasyPOS.Domain.Admin;
using EasyPOS.Domain.Common;
using EasyPOS.Domain.Products;
using EasyPOS.Domain.ProductTransfers;
using EasyPOS.Domain.Purchases;
using EasyPOS.Domain.Quotations;
using EasyPOS.Domain.Sales;
using EasyPOS.Domain.Settings;
using EasyPOS.Domain.Stakeholders;

namespace EasyPOS.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    //public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    #region Admin

    public DbSet<AppMenu> AppMenus => Set<AppMenu>();
    public DbSet<RoleMenu> RoleMenus => Set<RoleMenu>();
    public DbSet<AppPage> AppPages => Set<AppPage>();
    public DbSet<AppNotification> AppNotifications => Set<AppNotification>();
    #endregion

    #region Common
    public DbSet<Lookup> Lookups => Set<Lookup>();

    public DbSet<LookupDetail> LookupDetails => Set<LookupDetail>();

    #endregion

    #region Products
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Tax> Taxes => Set<Tax>();
    public DbSet<ProductAdjustment> ProductAdjustments => Set<ProductAdjustment>();
    public DbSet<ProductAdjustmentDetail> ProductAdjustmentDetails => Set<ProductAdjustmentDetail>();
    public DbSet<CountStock> CountStocks => Set<CountStock>();
    public DbSet<CountStockCategory> CountStockCategories => Set<CountStockCategory>();
    public DbSet<CountStockBrand> CountStockBrands => Set<CountStockBrand>();

    #endregion

    #region Product Transfer
    public DbSet<ProductTransfer> ProductTransfers => Set<ProductTransfer>();
    public DbSet<ProductTransferDetail> ProductTransferDetails => Set<ProductTransferDetail>();
    #endregion

    #region Stakeholders
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerGroup> CustomerGroups => Set<CustomerGroup>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    #endregion

    #region Purchase
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();
    public DbSet<PurchasePayment> PurchasePayments => Set<PurchasePayment>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnDetail> PurchaseReturnDetails => Set<PurchaseReturnDetail>();
    public DbSet<PurchaseReturnPayment> PurchaseReturnPayments => Set<PurchaseReturnPayment>();

    #endregion

    #region Sales
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();
    public DbSet<SalePayment> SalePayments => Set<SalePayment>();
    public DbSet<SaleReturn> SaleReturns => Set<SaleReturn>();
    public DbSet<SaleReturnPayment> SaleReturnPayments => Set<SaleReturnPayment>();
    public DbSet<SaleReturnDetail> SaleReturnDetails => Set<SaleReturnDetail>();

    public DbSet<Courier> Couriers => Set<Courier>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<GiftCard> GiftCards => Set<GiftCard>();
    #endregion

    #region Quotations
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationDetail> QuotationDetails => Set<QuotationDetail>();
    #endregion

    #region Accounting
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<MoneyTransfer> MoneyTransfers => Set<MoneyTransfer>();
    public DbSet<Expense> Expenses => Set<Expense>();

    #endregion

    #region Settings
    public DbSet<CompanyInfo> CompanyInfos => Set<CompanyInfo>();

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
