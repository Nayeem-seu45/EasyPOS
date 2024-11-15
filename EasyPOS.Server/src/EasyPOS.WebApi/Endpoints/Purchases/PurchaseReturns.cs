using EasyPOS.Application.Features.Common.Queries;
using EasyPOS.Application.Features.ProductManagement.Queries;
using EasyPOS.Application.Features.PurchaseReturns.Commands;
using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Application.Features.PurchaseReturns.Queries;
using EasyPOS.Application.Features.UnitManagement.Queries;

namespace EasyPOS.WebApi.Endpoints.PurchaseReturns;

public class PurchaseReturns : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
             .WithName("GetPurchaseReturns")
             .Produces<PaginatedResponse<PurchaseReturnModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id:Guid}", Get)
             .WithName("GetPurchaseReturn")
             .Produces<PurchaseReturnModel>(StatusCodes.Status200OK);

        group.MapGet("GetByPurchaseId/{purchaseId:Guid}", GetByPurchaseId)
             .WithName("GetByPurchaseId")
             .Produces<PurchaseReturnModel>(StatusCodes.Status200OK);

        group.MapGet("GetDetail/{id:Guid}", GetDetail)
             .WithName("GetPurchaseReturnDetail")
             .Produces<PurchaseReturnInfoModel>(StatusCodes.Status200OK);


        group.MapPost("Create", Create)
             .WithName("CreatePurchaseReturn")
             .Produces<Guid>(StatusCodes.Status201Created)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
             .WithName("UpdatePurchaseReturn")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id:Guid}", Delete)
             .WithName("DeletePurchaseReturn")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteMultiple", DeleteMultiple)
             .WithName("DeletePurchaseReturns")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("Upload", Upload)
             .WithName("PurchaseReturnUpload")
             .Produces<int>(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeletePurchaseReturnDetail", DeletePurchaseReturnDetail)
             .WithName("DeletePurchaseReturnDetail")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> GetAll(ISender sender, GetPurchaseReturnListQuery query)
    {
        var result = await sender.Send(query);
        if (!query.IsInitialLoaded)
        {
        }
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetPurchaseReturnByIdQuery(id));

        var warehousesSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.WarehouseSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Warehouse_All_SelectList}",
            AllowCacheList: true)
        );

        var suppliersSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetSupplierSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Supplier_All_SelectList}",
            AllowCacheList: true)
        );

        var purchaseStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = LookupDevCode.PurchaseReturnStatus },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.PurchaseReturnStatus}",
            AllowCacheList: false)
        );

        var taxesSelectList = await sender.Send(new GetSelectListQuery(
           Sql: SelectListSqls.TaxesSelectListSql,
           Parameters: new { },
           Key: CacheKeys.Tax_All_SelectList,
           AllowCacheList: true)
        );

        var returnStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = LookupDevCode.PurchaseReturnStatus },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.PurchaseReturnStatus}",
            AllowCacheList: false)
        );


        var productsSelectList = await sender.Send(new GetProductSelectListQuery(
            AllowCacheList: false)
        );

        var productUnitSelectList = await sender.Send(new GetUnitSelectListQuery(
           AllowCacheList: false)
        );

        result.Value.OptionsDataSources.Add("returnStatusSelectList", returnStatusSelectList.Value);
        result.Value.OptionsDataSources.Add("suppliersSelectList", suppliersSelectList.Value);
        result.Value.OptionsDataSources.Add("warehousesSelectList", warehousesSelectList.Value);
        result.Value.OptionsDataSources.Add("purchaseStatusSelectList", purchaseStatusSelectList.Value);
        result.Value.OptionsDataSources.Add("productsSelectList", productsSelectList.Value);
        result.Value.OptionsDataSources.Add("taxesSelectList", taxesSelectList.Value);
        result.Value.OptionsDataSources.Add("productUnitSelectList", productUnitSelectList.Value);



        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> GetByPurchaseId(ISender sender, Guid purchaseId)
    {
        var result = await sender.Send(new GetPurchaseReturnByPurchaseIdQuery(purchaseId));

        var warehousesSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.WarehouseSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Warehouse_All_SelectList}",
            AllowCacheList: true)
        );

        var suppliersSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetSupplierSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Supplier_All_SelectList}",
            AllowCacheList: true)
        );

        var returnStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = LookupDevCode.PurchaseReturnStatus },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.PurchaseReturnStatus}",
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

        result.Value.OptionsDataSources.Add("suppliersSelectList", suppliersSelectList.Value);
        result.Value.OptionsDataSources.Add("warehousesSelectList", warehousesSelectList.Value);
        result.Value.OptionsDataSources.Add("returnStatusSelectList", returnStatusSelectList.Value);
        result.Value.OptionsDataSources.Add("productsSelectList", productsSelectList.Value);
        result.Value.OptionsDataSources.Add("taxesSelectList", taxesSelectList.Value);
        result.Value.OptionsDataSources.Add("productUnitSelectList", productUnitSelectList.Value);



        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> GetDetail(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetPurchaseReturnDetailByIdQuery(id));

        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreatePurchaseReturnCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetPurchaseReturn", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdatePurchaseReturnCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeletePurchaseReturnCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> DeleteMultiple(ISender sender, [FromBody] Guid[] ids)
    {
        Result? result = null;
        foreach (var id in ids)
        {
            result = await sender.Send(new DeletePurchaseReturnCommand(id));
        }
        return result!.Match(
            onSuccess: Results.NoContent,
            onFailure: result!.ToProblemDetails);
    }

    private async Task<IResult> Upload(ISender sender, IHttpContextAccessor contextAccessor)
    {
        var file = contextAccessor.HttpContext.Request.Form.Files[0];

        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("No file uploaded.");
        }

        var result = await sender.Send(new CreatePurchaseReturnFromExcelCommand(file));

        return result!.Match(
            onSuccess: () => Results.Ok(result.Value),
            onFailure: result!.ToProblemDetails);
    }

    private async Task<IResult> DeletePurchaseReturnDetail(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeletePurchaseReturnDetailCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }
}
