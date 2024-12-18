﻿using EasyPOS.Application.Features.Common.Queries;
using EasyPOS.Application.Features.HRM.LeaveRequests.Commands;
using EasyPOS.Application.Features.HRM.LeaveRequests.Queries;
using EasyPOS.Application.Features.HRM.LeaveTypes.Queries;

namespace EasyPOS.WebApi.Endpoints;

public class LeaveRequests : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
             .WithName("GetLeaveRequests")
             .Produces<PaginatedResponse<LeaveRequestModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id:Guid}", Get)
             .WithName("GetLeaveRequest")
             .Produces<LeaveRequestModel>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
             .WithName("CreateLeaveRequest")
             .Produces<Guid>(StatusCodes.Status201Created)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
             .WithName("UpdateLeaveRequest")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id:Guid}", Delete)
             .WithName("DeleteLeaveRequest")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteMultiple", DeleteMultiple)
             .WithName("DeleteLeaveRequests")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("GetDateRangeTotalCount", GetDateRangeTotalCount)
             .WithName("GetDateRangeTotalCount")
             .Produces<int>(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Approval", Approval)
             .WithName("LeaveRequestApproval")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);


    }

    private async Task<IResult> GetAll(ISender sender, GetLeaveRequestListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetLeaveRequestByIdQuery(id));
        
        var employeesSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.EmployeesSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Employee}",
            AllowCacheList: true)
        );

        var leaveStatusSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
            Parameters: new { DevCode = (int)LookupDevCode.LeaveStatus },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.LeaveStatus}",
            AllowCacheList: true)
        );

        var leaveTypeSelectList = await sender.Send(new GetEmployeeLeaveTypeSelectListQuery(result.Value.EmployeeId, CacheAllowed: false));

        result.Value.OptionsDataSources.Add("employeesSelectList", employeesSelectList.Value);
        result.Value.OptionsDataSources.Add("leaveTypeSelectList", leaveTypeSelectList.Value);
        result.Value.OptionsDataSources.Add("leaveStatusSelectList", leaveStatusSelectList.Value);

        return TypedResults.Ok(result.Value);
    }



    private async Task<IResult> Create(ISender sender, [FromBody] CreateLeaveRequestCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetLeaveRequest", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateLeaveRequestCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteLeaveRequestCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> DeleteMultiple(ISender sender, [FromBody] Guid[] ids)
    {
        var result = await sender.Send(new DeleteLeaveRequestsCommand(ids));
        
        return result!.Match(
            onSuccess: Results.NoContent,
            onFailure: result!.ToProblemDetails);
    }

    private async Task<IResult> Approval(ISender sender, [FromBody] LeaveRequestApprovalCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> GetDateRangeTotalCount(ISender sender, [FromBody] GetDateRangeTotalCountQuery command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => TypedResults.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }
}
