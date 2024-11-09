using static EasyPOS.Application.Common.Security.Permissions;

namespace EasyPOS.Application.Common.Abstractions.Caching;

public static class CacheKeys
{
    public const string Lookup = nameof(Lookup);
    public const string Lookup_All_SelectList = nameof(Lookup_All_SelectList);

    public const string LookupDetail = nameof(LookupDetail);
    public const string LookupDetail_All_SelectList = nameof(LookupDetail_All_SelectList);

    #region Admin

    public const string AppUser = nameof(AppUser);
    public const string Role = nameof(Role);
    public const string Role_Permissions = nameof(Role_Permissions);
    public const string Role_All_SelectList = nameof(Role_All_SelectList);
    public const string AppMenu = nameof(AppMenu);
    public const string AppMenu_All_SelectList = nameof(AppMenu_All_SelectList);
    public const string AppMenu_Parent_SelectList = nameof(AppMenu_Parent_SelectList);
    public const string AppMenu_Tree_SelectList = nameof(AppMenu_Tree_SelectList);
    public const string AppMenu_Sidebar_Tree_List = nameof(AppMenu_Sidebar_Tree_List);
    public const string AppPage = nameof(AppPage);
    public const string AppPage_All_SelectList = nameof(AppPage_All_SelectList);
    public const string AppNotification = nameof(AppNotification);

    #endregion

    #region Products
    public const string Category = nameof(Category);
    public const string Category_All_SelectList = nameof(Category_All_SelectList);
    public const string Product = nameof(Product);
    public const string Product_All_SelectList = nameof(Product_All_SelectList);
    public const string Warehouse = nameof(Warehouse);
    public const string Warehouse_All_SelectList = nameof(Warehouse_All_SelectList);
    public const string Brand = nameof(Brand);
    public const string Brand_All_SelectList = nameof(Brand_All_SelectList);
    public const string Unit = nameof(Unit);
    public const string Unit_All_SelectList = nameof(Unit_All_SelectList);
    public const string Tax = nameof(Tax);
    public const string Tax_All_SelectList = nameof(Tax_All_SelectList);
    public const string ProductAdjustment = nameof(ProductAdjustment);
    public const string ProductAdjustment_All_SelectList = nameof(ProductAdjustment_All_SelectList);
    public const string CountStock = nameof(CountStock);
    #endregion

    #region Product Transfers
    public const string ProductTransfer = nameof(ProductTransfer);
    #endregion

    #region Stakeholders
    public const string Customer = nameof(Customer);
    public const string Customer_All_SelectList = nameof(Customer_All_SelectList);
    public const string CustomerGroup = nameof(CustomerGroup);
    public const string CustomerGroup_All_SelectList = nameof(CustomerGroup_All_SelectList);

    public const string Supplier = nameof(Supplier);
    public const string Supplier_All_SelectList = nameof(Supplier_All_SelectList);

    #endregion

    #region Purchase
    public const string Purchase = nameof(Purchase);
    public const string PurchasePayment = nameof(PurchasePayment);
    public const string PurchasePayment_PurchaseId = nameof(PurchasePayment_PurchaseId);
    public const string PurchaseReturn = nameof(PurchaseReturn);

    #endregion

    #region Sales

    public const string Sale = nameof(Sale);
    public const string SalePayment = nameof(SalePayment);
    public const string Courier = nameof(Courier);
    public const string Coupon = nameof(Coupon);
    public const string GiftCard = nameof(GiftCard);
    public const string SaleReturn = nameof(SaleReturn);

    #endregion

    #region Accounting
    public const string Account = nameof(Account);
    public const string Account_All_SelectList = nameof(Account_All_SelectList);
    public const string MoneyTransfer = nameof(MoneyTransfer);
    public const string MoneyTransfer_All_SelectList = nameof(MoneyTransfer_All_SelectList);
    public const string Expense = nameof(Expense);
    public const string Expense_All_SelectList = nameof(Expense_All_SelectList);


    #endregion

    #region Quotations
    public const string Quotation = nameof(Quotation);
    #endregion

    #region Settings
    public const string CompanyInfo = nameof(CompanyInfo);
    #endregion
}
