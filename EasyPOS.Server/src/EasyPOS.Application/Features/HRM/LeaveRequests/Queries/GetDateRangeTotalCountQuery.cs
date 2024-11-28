using System.Data;

namespace EasyPOS.Application.Features.HRM.LeaveRequests.Queries;

public record GetDateRangeTotalCountQuery(
    Guid EmployeeId,
    Guid LeaveTypeId,
    DateOnly StartDate,
    DateOnly EndDate) : ICacheableQuery<int>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.LeaveRequest}_{EmployeeId}_{LeaveTypeId}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetDateRangeTotalQueryHandler(
    ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetDateRangeTotalCountQuery, int>
{

    public async Task<Result<int>> Handle(GetDateRangeTotalCountQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnection.GetOpenConnection();

        var leaveType = await GetLeaveTypeAsync(connection, request.LeaveTypeId);

        if (leaveType == null)
            return Result.Failure<int>(Error.Failure(ErrorMessages.NotFound, "Leave Type not found."));

        var shiftDetails = await GetWorkingShiftDetailsAsync(connection, request.EmployeeId);

        if (!shiftDetails.Any())
            return Result.Failure<int>(Error.Failure(ErrorMessages.NotFound, "Working Shift not found."));

        var holidays = await GetHolidaysAsync(connection, request.StartDate, request.EndDate);

        // Calculate total days (inclusive of both start and end dates)
        int totalDays = CalculateTotalDays(request.StartDate, request.EndDate);

        // Handle Sandwich Leave Scenario
        if (leaveType.IsSandwichAllowed)
        {
            return ValidateSandwichLeave(leaveType.MaxConsecutiveDays, totalDays);
        }

        // Calculate Working Days
        var workingDays = CalculateWorkingDays(request.StartDate, totalDays, shiftDetails, holidays);

        // Validate against Max Consecutive Days
        if (leaveType.MaxConsecutiveDays != null && workingDays > leaveType.MaxConsecutiveDays)
        {
            return Result.Failure<int>(Error.Failure(ErrorMessages.InvalidOperation,
                "Selected range can't exceed Max Consecutive Days."));
        }

        return workingDays;
    }

    private static async Task<dynamic> GetLeaveTypeAsync(IDbConnection connection, Guid leaveTypeId)
    {
        return await connection.QueryFirstOrDefaultAsync<dynamic>(
            @"SELECT IsSandwichAllowed, MaxConsecutiveDays
              FROM LeaveTypes
              WHERE Id = @LeaveTypeId",
            new { LeaveTypeId = leaveTypeId });
    }

    private static async Task<IEnumerable<dynamic>> GetWorkingShiftDetailsAsync(IDbConnection connection, Guid employeeId)
    {
        return await connection.QueryAsync<dynamic>(
            @"SELECT DayOfWeek, IsWeekend
              FROM dbo.WorkingShiftDetails
              WHERE WorkingShiftId = (
                  SELECT ws.Id 
                  FROM dbo.Employees e 
                  INNER JOIN dbo.WorkingShifts ws ON ws.Id = e.WorkingShiftId
                  WHERE e.Id = @EmployeeId
              )",
            new { EmployeeId = employeeId });
    }

    private static async Task<IEnumerable<dynamic>> GetHolidaysAsync(
        IDbConnection connection, 
        DateOnly startDate, 
        DateOnly endDate)
    {
        return await connection.QueryAsync<dynamic>(
            @"SELECT StartDate, EndDate
              FROM Holidays
              WHERE IsActive = 1
                AND StartDate <= @EndDate
                AND EndDate >= @StartDate",
            new { StartDate = startDate, EndDate = endDate });
    }

    private static int CalculateTotalDays(
        DateOnly startDate,
        DateOnly endDate) => endDate.DayNumber - startDate.DayNumber + 1;

    private static Result<int> ValidateSandwichLeave(int? maxConsecutiveDays, int totalDays)
    {
        if (maxConsecutiveDays is not null && totalDays > maxConsecutiveDays)
        {
            return Result.Failure<int>(Error.Failure(
                ErrorMessages.InvalidOperation, "Selected range can't exceed Max Consecutive Days."));
        }
        return Result.Success(totalDays);
    }

    private static int CalculateWorkingDays(
        DateOnly startDate, 
        int totalDays, 
        IEnumerable<dynamic> shiftDetails, 
        IEnumerable<dynamic> holidays)
    {
        var dateRange = Enumerable.Range(0, totalDays)
                                  .Select(offset => startDate.AddDays(offset))
                                  .ToList();

        return dateRange.Count(date =>
            !IsWeekend(date, shiftDetails) &&
            !IsHoliday(date, holidays));
    }

    private static bool IsWeekend(DateOnly date, IEnumerable<dynamic> shiftDetails) 
        => shiftDetails.Any(sd => (DayOfWeek)sd.DayOfWeek == date.DayOfWeek && sd.IsWeekend);

    private static bool IsHoliday(
        DateOnly date, 
        IEnumerable<dynamic> holidays) => holidays.Any(h => h.StartDate <= date && h.EndDate >= date);
}
