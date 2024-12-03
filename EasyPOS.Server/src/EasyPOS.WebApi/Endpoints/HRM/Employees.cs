using EasyPOS.Application.Common.Models;
using EasyPOS.Application.Features.Admin.AppUsers.Queries;
using EasyPOS.Application.Features.Common.Queries;
using EasyPOS.Application.Features.HRM.Employees.Commands;
using EasyPOS.Application.Features.HRM.Employees.Queries;
using EasyPOS.Application.Features.HRM.LeaveTypes.Queries;

namespace EasyPOS.WebApi.Endpoints;

public class Employees : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
             .WithName("GetEmployees")
             .Produces<PaginatedResponse<EmployeeModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id:Guid}", Get)
             .WithName("GetEmployee")
             .Produces<EmployeeModel>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
             .WithName("CreateEmployee")
             .Produces<Guid>(StatusCodes.Status201Created)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
             .WithName("UpdateEmployee")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id:Guid}", Delete)
             .WithName("DeleteEmployee")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteMultiple", DeleteMultiple)
             .WithName("DeleteEmployees")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapGet("GetEmployeeHierarchy/{id:Guid}", GetEmployeeHierarchy)
             .WithName("GetEmployeeHierarchy")
             .Produces<HierarchyTreeNodeModel>(StatusCodes.Status200OK);

    }

    private async Task<IResult> GetAll(ISender sender, GetEmployeeListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetEmployeeByIdQuery(id));

        var departmentSelectList = await sender.Send(new GetSelectListQuery(
           Sql: SelectListSqls.DepartmentsSelectListSql,
           Parameters: new { },
           Key: CacheKeys.Department_All_SelectList,
           AllowCacheList: false)
        );

        var designationSelectList = await sender.Send(new GetSelectListQuery(
           Sql: SelectListSqls.DesignationsSelectListSql,
           Parameters: new { },
           Key: CacheKeys.Designation_All_SelectList,
           AllowCacheList: false)
        );

        var workingShiftSelectList = await sender.Send(new GetSelectListQuery(
           Sql: SelectListSqls.WorkingShiftsSelectListSql,
           Parameters: new { },
           Key: CacheKeys.WorkingShift_All_SelectList,
           AllowCacheList: false)
        );


        var warehousesSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.WarehouseSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Warehouse_All_SelectList}",
            AllowCacheList: true)
        );

        var countrySelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.LookupDetailNameKeySelectListByDevCodeSql,
            Parameters: new { DevCode = (int)LookupDevCode.Country },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.BarCodeSymbol}",
            AllowCacheList: true)
        );

        var genderSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.LookupDetailNameKeySelectListByDevCodeSql,
            Parameters: new { DevCode = (int)LookupDevCode.Gender },
            Key: $"{CacheKeys.LookupDetail}_{LookupDevCode.Gender}",
            AllowCacheList: true)
        );

        var employeesSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.EmployeesSelectListSql,
            Parameters: new { },
            Key: $"{CacheKeys.Employee}",
            AllowCacheList: true)
        );

        var leaveTypeSelectList = await sender.Send(new GetLeaveTypeSelectListQuery(CacheAllowed: false));
        var userSelectList = await sender.Send(new GetAppUserSelectListQuery());

        result.Value.OptionsDataSources.Add("employeesSelectList", employeesSelectList.Value);
        result.Value.OptionsDataSources.Add("warehousesSelectList", warehousesSelectList.Value);
        result.Value.OptionsDataSources.Add("departmentSelectList", departmentSelectList.Value);
        result.Value.OptionsDataSources.Add("designationSelectList", designationSelectList.Value);
        result.Value.OptionsDataSources.Add("workingShiftSelectList", workingShiftSelectList.Value);
        result.Value.OptionsDataSources.Add("countrySelectList", countrySelectList.Value);
        result.Value.OptionsDataSources.Add("genderSelectList", genderSelectList.Value);
        result.Value.OptionsDataSources.Add("leaveTypeSelectList", leaveTypeSelectList.Value);
        result.Value.OptionsDataSources.Add("userSelectList", userSelectList.Value);


        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateEmployeeCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetEmployee", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateEmployeeCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteEmployeeCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> DeleteMultiple(ISender sender, [FromBody] Guid[] ids)
    {
        var result = await sender.Send(new DeleteEmployeesCommand(ids));

        return result!.Match(
            onSuccess: Results.NoContent,
            onFailure: result!.ToProblemDetails);
    }

    private async Task<IResult> GetEmployeeHierarchy(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetEmployeeSubordinateByIdQuery(id));

        return result.Match(
            onSuccess: () => Results.Ok(result.Value),
            onFailure: result.ToProblemDetails);


    }
}
