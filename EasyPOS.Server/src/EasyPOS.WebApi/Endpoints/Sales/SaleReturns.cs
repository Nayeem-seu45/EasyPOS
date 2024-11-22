using EasyPOS.Application.Features.Common.Queries;
using EasyPOS.Application.Features.ProductManagement.Products.SelectLists;
using EasyPOS.Application.Features.ProductManagement.Queries;
using EasyPOS.Application.Features.SaleReturns.Commands;
using EasyPOS.Application.Features.SaleReturns.Models;
using EasyPOS.Application.Features.SaleReturns.Queries;
using EasyPOS.Application.Features.UnitManagement.Queries;

namespace EasyPOS.WebApi.Endpoints.SaleReturns;

public class SaleReturns : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
             .WithName("GetSaleReturns")
             .Produces<PaginatedResponse<SaleReturnModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id:Guid}", Get)
             .WithName("GetSaleReturn")
             .Produces<UpsertSaleReturnModel>(StatusCodes.Status200OK);

        group.MapGet("GetBySaleId/{saleId:Guid}", GetBySaleId)
             .WithName("GetSaleReturnBySaleId")
             .Produces<UpsertSaleReturnModel>(StatusCodes.Status200OK);


        group.MapGet("GetDetail/{id:Guid}", GetDetail)
             .WithName("GetSaleReturnDetail")
             .Produces<SaleReturnInfoModel>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
             .WithName("CreateSaleReturn")
             .Produces<Guid>(StatusCodes.Status201Created)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
             .WithName("UpdateSaleReturn")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id:Guid}", Delete)
             .WithName("DeleteSaleReturn")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteMultiple", DeleteMultiple)
             .WithName("DeleteSaleReturns")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteSaleReturnDetail", DeleteSaleReturnDetail)
             .WithName("DeleteSaleReturnDetail")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

    }

    private async Task<IResult> GetAll(ISender sender, GetSaleReturnListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetSaleReturnByIdQuery(id));

        var warehousesSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.WarehouseSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Warehouse_All_SelectList}",
            AllowCacheList: true)
        );

        var customersSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetCustomerSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Customer_All_SelectList}",
            AllowCacheList: true)
        );

        var saleStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = LookupDevCode.SaleReturnStatus },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.SaleReturnStatus}",
            AllowCacheList: false)
        );

        var paymentStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = LookupDevCode.PurchasePaymentStatus },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.PurchasePaymentStatus}",
            AllowCacheList: false)
        );


        var taxesSelectList = await sender.Send(new GetSelectListQuery(
           Sql: SelectListSqls.TaxesSelectListSql,
           Parameters: new { },
           Key: CacheKeys.Tax_All_SelectList,
           AllowCacheList: true)
        );

        var productsSelectList = await sender.Send(new GetProductSelectListQuery(
            AllowCacheList: false)
        );

        var productUnitSelectList = await sender.Send(new GetUnitSelectListQuery(
           AllowCacheList: false)
        );

        result.Value.OptionsDataSources.Add("customersSelectList", customersSelectList.Value);
        result.Value.OptionsDataSources.Add("warehousesSelectList", warehousesSelectList.Value);
        result.Value.OptionsDataSources.Add("saleStatusSelectList", saleStatusSelectList.Value);
        result.Value.OptionsDataSources.Add("paymentStatusSelectList", paymentStatusSelectList.Value);
        result.Value.OptionsDataSources.Add("productsSelectList", productsSelectList.Value);
        result.Value.OptionsDataSources.Add("taxesSelectList", taxesSelectList.Value);
        result.Value.OptionsDataSources.Add("productUnitSelectList", productUnitSelectList.Value);

        return TypedResults.Ok(result.Value);
    }


    private async Task<IResult> GetBySaleId(ISender sender, Guid saleId)
    {
        var result = await sender.Send(new GetSaleReturnBySaleIdQuery(saleId));

        var warehousesSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.WarehouseSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Warehouse_All_SelectList}",
            AllowCacheList: true)
        );

        var customersSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetCustomerSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Customer_All_SelectList}",
            AllowCacheList: true)
        );

        var saleStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = LookupDevCode.SaleReturnStatus },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.SaleReturnStatus}",
            AllowCacheList: false)
        );

        var paymentStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = LookupDevCode.PurchasePaymentStatus },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.PurchasePaymentStatus}",
            AllowCacheList: false)
        );


        var taxesSelectList = await sender.Send(new GetSelectListQuery(
           Sql: SelectListSqls.TaxesSelectListSql,
           Parameters: new { },
           Key: CacheKeys.Tax_All_SelectList,
           AllowCacheList: true)
        );

        var productsSelectList = await sender.Send(new GetProductSelectListQuery(
            AllowCacheList: false)
        );

        var productUnitSelectList = await sender.Send(new GetUnitSelectListQuery(
           AllowCacheList: false)
        );

        result.Value.OptionsDataSources.Add("customersSelectList", customersSelectList.Value);
        result.Value.OptionsDataSources.Add("warehousesSelectList", warehousesSelectList.Value);
        result.Value.OptionsDataSources.Add("saleStatusSelectList", saleStatusSelectList.Value);
        result.Value.OptionsDataSources.Add("paymentStatusSelectList", paymentStatusSelectList.Value);
        result.Value.OptionsDataSources.Add("productsSelectList", productsSelectList.Value);
        result.Value.OptionsDataSources.Add("taxesSelectList", taxesSelectList.Value);
        result.Value.OptionsDataSources.Add("productUnitSelectList", productUnitSelectList.Value);

        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> GetDetail(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetSaleReturnDetailByIdQuery(id));

        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateSaleReturnCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetSaleReturn", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateSaleReturnCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteSaleReturnCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> DeleteMultiple(ISender sender, [FromBody] Guid[] ids)
    {
        var result = await sender.Send(new DeleteSaleReturnsCommand(ids));

        return result!.Match(
            onSuccess: Results.NoContent,
            onFailure: result!.ToProblemDetails);
    }

    private async Task<IResult> DeleteSaleReturnDetail(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteSaleReturnDetailCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

}
