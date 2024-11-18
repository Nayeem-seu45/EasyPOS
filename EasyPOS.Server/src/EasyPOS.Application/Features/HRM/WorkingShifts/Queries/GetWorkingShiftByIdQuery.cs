using EasyPOS.Application.Features.HRM.WorkingShifts.Models;

namespace EasyPOS.Application.Features.HRM.WorkingShifts.Queries;

public record GetWorkingShiftByIdQuery(Guid Id) : ICacheableQuery<WorkingShiftModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.WorkingShift}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetWorkingShiftByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetWorkingShiftByIdQuery, WorkingShiftModel>
{

    public async Task<Result<WorkingShiftModel>> Handle(GetWorkingShiftByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new WorkingShiftModel()
            {
                IsActive = true,
                WorkingShiftDetails = GetWeeklyShiftDetail()
            };
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                -- WorkingShift
                t.Id AS {nameof(WorkingShiftModel.Id)},
                t.ShiftName AS {nameof(WorkingShiftModel.ShiftName)},
                t.Description AS {nameof(WorkingShiftModel.Description)},
                t.IsActive AS {nameof(WorkingShiftModel.IsActive)},

                --WorkingShiftDetail
                wd.Id AS {nameof(WorkingShiftDetailModel.Id)},
                wd.WorkingShiftId AS {nameof(WorkingShiftDetailModel.WorkingShiftId)},
                wd.DayOfWeek AS {nameof(WorkingShiftDetailModel.DayOfWeek)},
                wd.StartTime AS {nameof(WorkingShiftDetailModel.StartTime)},
                wd.EndTime AS {nameof(WorkingShiftDetailModel.EndTime)},
                wd.IsWeekend AS {nameof(WorkingShiftDetailModel.IsWeekend)},
                CASE wd.DayOfWeek
                WHEN 0 THEN 'Sunday'
                WHEN 1 THEN 'Monday'
                WHEN 2 THEN 'Tuesday'
                WHEN 3 THEN 'Wednesday'
                WHEN 4 THEN 'Thursday'
                WHEN 5 THEN 'Friday'
                WHEN 6 THEN 'Saturday'
            END AS {nameof(WorkingShiftDetailModel.DayNameOfWeek)}
           
            FROM dbo.WorkingShifts AS t
            LEFT JOIN dbo.WorkingShiftDetails wd ON wd.WorkingShiftId = t.Id
            WHERE t.Id = @Id
            ORDER BY wd.DayOfWeek
            """;

        var workShiftDictionary = new Dictionary<Guid, WorkingShiftModel>();

        var workshiftModels = await connection.QueryAsync<WorkingShiftModel, WorkingShiftDetailModel, WorkingShiftModel>(
            sql,
            (workShift, workShiftDetail) =>
            {
                if (!workShiftDictionary.TryGetValue(workShift.Id, out var workingShiftModel))
                {
                    workingShiftModel = workShift;
                    workingShiftModel.WorkingShiftDetails = [];
                    workShiftDictionary.Add(workingShiftModel.Id, workingShiftModel);
                }

                if (workShift is not null)
                {
                    workingShiftModel.WorkingShiftDetails.Add(workShiftDetail);
                }

                return workingShiftModel;
            },
            new { request.Id },
            splitOn: nameof(WorkingShiftModel.Id)
        );

        var workshiftModel = workshiftModels.FirstOrDefault();

        return workshiftModel;

    }

    private List<WorkingShiftDetailModel> GetWeeklyShiftDetail()
    {
        return [
             new WorkingShiftDetailModel {DayOfWeek = DayOfWeek.Sunday, DayNameOfWeek = nameof(DayOfWeek.Sunday), IsWeekend = false},
             new WorkingShiftDetailModel {DayOfWeek = DayOfWeek.Monday, DayNameOfWeek = nameof(DayOfWeek.Monday), IsWeekend = false},
             new WorkingShiftDetailModel {DayOfWeek = DayOfWeek.Tuesday, DayNameOfWeek = nameof(DayOfWeek.Tuesday), IsWeekend = false},
             new WorkingShiftDetailModel {DayOfWeek = DayOfWeek.Wednesday, DayNameOfWeek = nameof(DayOfWeek.Wednesday), IsWeekend = false},
             new WorkingShiftDetailModel {DayOfWeek = DayOfWeek.Thursday, DayNameOfWeek = nameof(DayOfWeek.Thursday), IsWeekend = false},
             new WorkingShiftDetailModel {DayOfWeek = DayOfWeek.Friday, DayNameOfWeek = nameof(DayOfWeek.Friday), IsWeekend = false},
             new WorkingShiftDetailModel {DayOfWeek = DayOfWeek.Saturday, DayNameOfWeek = nameof(DayOfWeek.Saturday), IsWeekend = false}
        ];
    }
}

