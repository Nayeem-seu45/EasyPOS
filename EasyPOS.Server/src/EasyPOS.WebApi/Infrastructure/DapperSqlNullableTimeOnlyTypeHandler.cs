using Dapper;
using System.Data;

namespace EasyPOS.WebApi.Infrastructure;

public class DapperSqlNullableTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly?>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly? time)
    {
        // Convert nullable TimeOnly to TimeSpan for storage
        parameter.Value = time.HasValue ? time.Value.ToTimeSpan() : (object)DBNull.Value;
    }

    public override TimeOnly? Parse(object value)
    {
        // Convert stored TimeSpan back to nullable TimeOnly
        return value == DBNull.Value ? (TimeOnly?)null : TimeOnly.FromTimeSpan((TimeSpan)value);
    }
}

