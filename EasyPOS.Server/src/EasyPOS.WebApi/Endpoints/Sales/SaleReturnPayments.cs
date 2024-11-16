using EasyPOS.Application.Features.Common.Queries;
using EasyPOS.Application.Features.Sales.SaleReturnPayments.Commands;
using EasyPOS.Application.Features.Sales.SaleReturnPayments.Models;
using EasyPOS.Application.Features.Sales.SaleReturnPayments.Queries;

namespace EasyPOS.WebApi.Endpoints.SaleReturns;

public class SaleReturnPayments : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
             .WithName("GetSaleReturnPayments")
             .Produces<PaginatedResponse<SaleReturnPaymentModel>>(StatusCodes.Status200OK);

        group.MapPost("GetAllBySaleReturnId", GetAllBySaleReturnId)
             .WithName("GetAllBySaleReturnId")
             .Produces<List<SaleReturnPaymentModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id:Guid}", Get)
             .WithName("GetSaleReturnPayment")
             .Produces<SaleReturnPaymentModel>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
             .WithName("CreateSaleReturnPayment")
             .Produces<Guid>(StatusCodes.Status201Created)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
             .WithName("UpdateSaleReturnPayment")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id:Guid}", Delete)
             .WithName("DeleteSaleReturnPayment")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteMultiple", DeleteMultiple)
             .WithName("DeleteSaleReturnPayments")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> GetAll(ISender sender, GetSaleReturnPaymentListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> GetAllBySaleReturnId(ISender sender, GetPaymentListBySaleReturnIdQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetSaleReturnPaymentByIdQuery(id));

        var paymentTypeSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = (int)LookupDevCode.PaymentType },
            Key: $"{CacheKeys.LookupDetail}_{(int)LookupDevCode.PaymentType}",
            AllowCacheList: false)
        );

        result.Value.OptionsDataSources.Add("paymentTypeSelectList", paymentTypeSelectList.Value);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateSaleReturnPaymentCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetSaleReturnPayment", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateSaleReturnPaymentCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteSaleReturnPaymentCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> DeleteMultiple(ISender sender, [FromBody] Guid[] ids)
    {
        var result = await sender.Send(new DeleteSaleReturnPaymentsCommand(ids));

        return result!.Match(
            onSuccess: Results.NoContent,
            onFailure: result!.ToProblemDetails);
    }
}
