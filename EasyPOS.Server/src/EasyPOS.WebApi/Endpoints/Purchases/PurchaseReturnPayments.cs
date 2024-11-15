using EasyPOS.Application.Features.Common.Queries;
using EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Commands;
using EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Models;
using EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Queries;

namespace EasyPOS.WebApi.Endpoints.PurchaseReturns;

public class PurchaseReturnPayments : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
             .WithName("GetPurchaseReturnPayments")
             .Produces<PaginatedResponse<PurchaseReturnPaymentModel>>(StatusCodes.Status200OK);

        group.MapPost("GetAllByPurchaseReturnId", GetAllByPurchaseReturnId)
             .WithName("GetAllByPurchaseReturnId")
             .Produces<List<PurchaseReturnPaymentModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id:Guid}", Get)
             .WithName("GetPurchaseReturnPayment")
             .Produces<PurchaseReturnPaymentModel>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
             .WithName("CreatePurchaseReturnPayment")
             .Produces<Guid>(StatusCodes.Status201Created)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
             .WithName("UpdatePurchaseReturnPayment")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id:Guid}", Delete)
             .WithName("DeletePurchaseReturnPayment")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteMultiple", DeleteMultiple)
             .WithName("DeletePurchaseReturnPayments")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> GetAll(ISender sender, GetPurchaseReturnPaymentListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> GetAllByPurchaseReturnId(ISender sender, GetPaymentListByPurchaseReturnIdQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetPurchaseReturnPaymentByIdQuery(id));

        var purchaseStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = (int)LookupDevCode.PaymentType },
            Key: $"{CacheKeys.LookupDetail}_{(int)LookupDevCode.PaymentType}",
            AllowCacheList: false)
        );

        result.Value.OptionsDataSources.Add("paymentTypeSelectList", purchaseStatusSelectList.Value);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreatePurchaseReturnPaymentCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetPurchaseReturnPayment", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdatePurchaseReturnPaymentCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeletePurchaseReturnPaymentCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> DeleteMultiple(ISender sender, [FromBody] Guid[] ids)
    {
        var result = await sender.Send(new DeletePurchaseReturnPaymentsCommand(ids));

        return result!.Match(
            onSuccess: Results.NoContent,
            onFailure: result!.ToProblemDetails);
    }
}
