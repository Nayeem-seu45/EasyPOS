﻿using Dapper;
using System.Data;

namespace EasyPOS.WebApi.Infrastructure;

public class DapperSqlNullableDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly?>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly? date)
    {
        // Convert nullable DateOnly to DateTime for storage
        parameter.Value = date.HasValue ? date.Value.ToDateTime(new TimeOnly(0, 0)) : (object)DBNull.Value;
    }

    public override DateOnly? Parse(object value)
    {
        // Convert stored DateTime back to nullable DateOnly
        return value == DBNull.Value ? (DateOnly?)null : DateOnly.FromDateTime((DateTime)value);
    }
}

