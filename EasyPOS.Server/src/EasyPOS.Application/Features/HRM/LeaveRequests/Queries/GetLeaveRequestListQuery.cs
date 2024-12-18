﻿using EasyPOS.Application.Common.Enums;

namespace EasyPOS.Application.Features.HRM.LeaveRequests.Queries;

[Authorize(Policy = Permissions.LeaveRequests.View)]
public record GetLeaveRequestListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<LeaveRequestModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.LeaveRequest}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetLeaveRequestQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetLeaveRequestListQuery, PaginatedResponse<LeaveRequestModel>>
{
    public async Task<Result<PaginatedResponse<LeaveRequestModel>>> Handle(GetLeaveRequestListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(LeaveRequestModel.Id)},
                t.EmployeeId AS {nameof(LeaveRequestModel.EmployeeId)},
                t.LeaveTypeId AS {nameof(LeaveRequestModel.LeaveTypeId)},
                t.StartDate AS {nameof(LeaveRequestModel.StartDate)},
                t.EndDate AS {nameof(LeaveRequestModel.EndDate)},
                t.TotalDays AS {nameof(LeaveRequestModel.TotalDays)},
                t.StatusId AS {nameof(LeaveRequestModel.StatusId)},
                t.AttachmentUrl AS {nameof(LeaveRequestModel.AttachmentUrl)},
                t.Reason AS {nameof(LeaveRequestModel.Reason)},
                e.FirstName AS {nameof(LeaveRequestModel.EmployeeName)},
                d.Name AS {nameof(LeaveRequestModel.Department)},
                d2.Name AS {nameof(LeaveRequestModel.Designation)},
                ls.Name AS {nameof(LeaveRequestModel.Status)},
                CASE
                WHEN ls.DevCode IS NULL THEN NULL
                WHEN ISNULL(ls.DevCode, 0) >= {(int)LeaveStatus.Submitted} THEN 0
                ELSE 1
                END AS  {nameof(LeaveRequestModel.HideRemoveIfSubmitted)}
            FROM dbo.LeaveRequests AS t
            LEFT JOIN dbo.Employees e ON e.Id = t.EmployeeId
            LEFT JOIN dbo.Departments d ON d.Id = e.DepartmentId
            LEFT JOIN dbo.Designations d2 ON d2.Id = e.DesignationId
            LEFT JOIN dbo.LookupDetails ls ON ls.Id = t.StatusId
            WHERE 1 = 1
            """;


        return await PaginatedResponse<LeaveRequestModel>
            .CreateAsync(connection, sql, request);

    }
}


