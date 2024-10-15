﻿using System.Text;
using Dapper;
using EasyPOS.Application.Common.Abstractions;
using EasyPOS.Application.Common.DapperQueries;
using EasyPOS.Domain.Common;

namespace EasyPOS.Infrastructure.Persistence.Services;

internal sealed class CommonQueryService(ISqlConnectionFactory sqlConnection) : ICommonQueryService
{
    public async Task<Guid?> GetLookupDetailIdAsync(int lookupDetailDevCode)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = """
            SELECT TOP 1 Id
            FROM dbo.LookupDetails
            WHERE DevCode = @DevCode
            """;
        return await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new {DevCode = lookupDetailDevCode});
    }

    public async Task<List<LookupDetail>> GetLookupDetailsAsync(int lookupDevCode)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = """
            SELECT 
                ld.Id, 
                ld.Name, 
                ld.Description, 
                ld.Status, 
                ld.ParentId, 
                ld.LookupId, 
                ld.DevCode
            FROM 
                dbo.Lookups l
            INNER JOIN 
                dbo.LookupDetails ld ON l.Id = ld.LookupId
            WHERE 
                l.DevCode = @DevCode          
            """;

        return await connection.QueryFirstOrDefaultAsync<List<LookupDetail>>(sql, new { DevCode = lookupDevCode });
    }

    public async Task<bool> IsExistAsync(
        string tableName,
        string[]? equalFilters,
        object? param = null,
        string[]? notEqualFilters = null)
    {
        var connection = sqlConnection.GetOpenConnection();

        StringBuilder sql = new($"SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM {tableName} WHERE 1 = 1");

        foreach (var filter in equalFilters ??= [])
        {
            sql.Append($" AND {filter} = @{filter}");
        }

        foreach (var filter in notEqualFilters ??= [])
        {
            sql.Append($" AND {filter} <> @{filter}");
        }

        sql.Append(") THEN 1 ELSE 0 END AS BIT)");

        return await connection.ExecuteScalarAsync<bool>(sql.ToString(), param);
    }

}
