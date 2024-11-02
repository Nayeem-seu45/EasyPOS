using EasyPOS.Application.Features.Common.Queries;
using EasyPOS.Application.Features.ProductManagement.Queries;
using EasyPOS.Application.Features.Quotations.Commands;
using EasyPOS.Application.Features.Quotations.Queries;
using EasyPOS.Application.Features.UnitManagement.Queries;

namespace EasyPOS.WebApi.Endpoints.Quotations;

public class Quotations : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
             .WithName("GetQuotations")
             .Produces<PaginatedResponse<QuotationModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id:Guid}", Get)
             .WithName("GetQuotation")
             .Produces<UpsertQuotationModel>(StatusCodes.Status200OK);

        group.MapGet("GetDetail/{id:Guid}", GetDetail)
             .WithName("GetQuotationDetail")
             .Produces<QuotationInfoModel>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
             .WithName("CreateQuotation")
             .Produces<Guid>(StatusCodes.Status201Created)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
             .WithName("UpdateQuotation")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id:Guid}", Delete)
             .WithName("DeleteQuotation")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteMultiple", DeleteMultiple)
             .WithName("DeleteQuotations")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteQuotationDetail", DeleteQuotationDetail)
             .WithName("DeleteQuotationDetail")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

    }

    private async Task<IResult> GetAll(ISender sender, GetQuotationListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetQuotationByIdQuery(id));

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

        var quotationStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = (int)LookupDevCode.QuotationStatus },
            Key: $"{CacheKeys.LookupDetail}_{(int)LookupDevCode.QuotationStatus}",
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
        result.Value.OptionsDataSources.Add("quotationStatusSelectList", quotationStatusSelectList.Value);
        result.Value.OptionsDataSources.Add("productsSelectList", productsSelectList.Value);
        result.Value.OptionsDataSources.Add("taxesSelectList", taxesSelectList.Value);
        result.Value.OptionsDataSources.Add("productUnitSelectList", productUnitSelectList.Value);

        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> GetDetail(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetQuotationDetailByIdQuery(id));

        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateQuotationCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetQuotation", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateQuotationCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteQuotationCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> DeleteMultiple(ISender sender, [FromBody] Guid[] ids)
    {
        var result = await sender.Send(new DeleteQuotationsCommand(ids));

        return result!.Match(
            onSuccess: Results.NoContent,
            onFailure: result!.ToProblemDetails);
    }

    private async Task<IResult> DeleteQuotationDetail(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteQuotationDetailCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

}
