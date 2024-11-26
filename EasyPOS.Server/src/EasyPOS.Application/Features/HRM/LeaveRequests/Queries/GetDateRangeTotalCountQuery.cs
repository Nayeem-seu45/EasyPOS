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

internal sealed class GetDateRangeTotalQueryHandler(ISqlConnectionFactory sqlConnection, IApplicationDbContext applicationDbContext)
     : IQueryHandler<GetDateRangeTotalCountQuery, int>
{

    public async Task<Result<int>> Handle(GetDateRangeTotalCountQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnection.GetOpenConnection();

        // Query LeaveType
        var leaveType = await connection.QueryFirstOrDefaultAsync<dynamic>(
            @"SELECT IsSandwichAllowed, MaxConsecutiveDays
              FROM LeaveTypes
              WHERE Id = @LeaveTypeId",
            new { request.LeaveTypeId });

        if (leaveType == null)
        {
            return Result.Failure<int>(Error.Failure(ErrorMessages.NotFound, "Leave Type not found."));
        }


        // Query WorkingShiftDetails
        var shiftDetails = await connection.QueryAsync<dynamic>(
            @"SELECT DayOfWeek, IsWeekend
              FROM WorkingShiftDetails
              WHERE WorkingShiftId = (
                  SELECT Id FROM WorkingShifts WHERE EmployeeId = @EmployeeId
              )",
            new { request.EmployeeId });

        if (!shiftDetails.Any())
        {
            return Result.Failure<int>(Error.Failure(ErrorMessages.NotFound, "Working Shift not found."));
        }

        // Query Holidays
        var holidays = await connection.QueryAsync<dynamic>(
            @"SELECT StartDate, EndDate
              FROM Holidays
              WHERE IsActive = 1
                AND StartDate <= @EndDate
                AND EndDate >= @StartDate",
            new { request.StartDate, request.EndDate });

        // Calculate Total Days
        int totalDays = request.EndDate.DayNumber - request.StartDate.DayNumber;

        if (leaveType.IsSandwichAllowed)
        {
            return leaveType.MaxConsecutiveDays is not null && totalDays > (int)leaveType.MaxConsecutiveDays
                ? Result.Failure<int>(Error.Failure(ErrorMessages.InvalidOperation, "Selected range can't exceed Max Consecutive Days."))
                : (Result<int>)totalDays;
        }

        // Build Date Range
        var dateRange = Enumerable.Range(0, totalDays)
                                  .Select(offset => request.StartDate.AddDays(offset))
                                  .ToList();

        // Filter Weekends and Holidays
        int workingDays = dateRange.Count(date =>
            !shiftDetails.Any(sd => (DayOfWeek)sd.DayOfWeek == date.DayOfWeek && sd.IsWeekend) &&
            !holidays.Any(h => h.StartDate <= date && h.EndDate >= date));

        if (leaveType.MaxConsecutiveDays != null && workingDays > (int)leaveType.MaxConsecutiveDays)
        {
            return Result.Failure<int>(Error.Failure(
                ErrorMessages.InvalidOperation,
                "Selected range can't exceed Max Consecutive Days."));
        }

        return workingDays;
    }
}
