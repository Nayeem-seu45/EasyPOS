using System.Text;
using Dapper;
using EasyPOS.Application.Common.Abstractions;
using EasyPOS.Application.Common.DapperQueries;
using EasyPOS.Application.Features.Customers.Models;
using EasyPOS.Application.Features.Settings.CompanyInfos.Queries;
using EasyPOS.Application.Features.Suppliers.Models;
using EasyPOS.Domain.Common;

namespace EasyPOS.Infrastructure.Persistence.Services;

internal sealed class CommonQueryService(ISqlConnectionFactory sqlConnection) : ICommonQueryService
{
    public async Task<Guid?> GetLookupDetailIdAsync(
        int lookupDetailDevCode, 
        CancellationToken cancellationToken = default)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = """
            SELECT TOP 1 Id
            FROM dbo.LookupDetails
            WHERE DevCode = @DevCode
            """;
        return await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new {DevCode = lookupDetailDevCode});
    }

    public async Task<List<LookupDetail>> GetLookupDetailsAsync(
        int lookupDevCode, 
        CancellationToken cancellationToken = default)
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

        var result = await connection.QueryAsync<LookupDetail>(sql, new { DevCode = lookupDevCode });

        return result.AsList();
    }

    public async Task<SupplierModel> GetSupplierDetail(Guid supplierId, CancellationToken cancellationToken = default)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(SupplierModel.Id)},
                t.Name AS {nameof(SupplierModel.Name)},
                ISNULL(t.Email, '') AS {nameof(SupplierModel.Email)},
                ISNULL(t.PhoneNo, '') AS {nameof(SupplierModel.PhoneNo)},
                ISNULL(t.Mobile, '') AS {nameof(SupplierModel.Mobile)},
                ISNULL(t.Country, '') AS {nameof(SupplierModel.Country)},
                ISNULL(t.City, '') AS {nameof(SupplierModel.City)},
                ISNULL(t.Address, '') AS {nameof(SupplierModel.Address)},
                t.IsActive AS {nameof(SupplierModel.IsActive)}
            FROM dbo.Suppliers t
            WHERE t.Id = @Id
            """
        ;
        return await connection.QueryFirstOrDefaultAsync<SupplierModel>(sql, new {Id = supplierId });
    }

    public async Task<CustomerModel> GetCustomerDetail(Guid customerId, CancellationToken cancellationToken = default)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(CustomerModel.Id)},
                t.Name AS {nameof(CustomerModel.Name)},
                ISNULL(t.Email, '') AS {nameof(CustomerModel.Email)},
                ISNULL(t.PhoneNo, '') AS {nameof(CustomerModel.PhoneNo)},
                ISNULL(t.Mobile, '') AS {nameof(CustomerModel.Mobile)},
                ISNULL(t.Country, '') AS {nameof(CustomerModel.Country)},
                ISNULL(t.City, '') AS {nameof(CustomerModel.City)},
                ISNULL(t.Address, '') AS {nameof(CustomerModel.Address)},
                t.IsActive AS {nameof(CustomerModel.IsActive)}
            FROM dbo.Customers t
            WHERE t.Id = @Id
            """
        ;
        return await connection.QueryFirstOrDefaultAsync<CustomerModel>(sql, new { Id = customerId });
    }

    public async Task<CompanyInfoModel> GetCompanyInfoAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT TOP 1
                t.Id AS {nameof(CompanyInfoModel.Id)},
                ISNULL(t.Name, '') AS {nameof(CompanyInfoModel.Name)},
                ISNULL(t.Phone, '') AS {nameof(CompanyInfoModel.Phone)},
                ISNULL(t.Mobile, '') AS {nameof(CompanyInfoModel.Mobile)},
                ISNULL(t.Email, '') AS {nameof(CompanyInfoModel.Email)},
                ISNULL(t.Country, '') AS {nameof(CompanyInfoModel.Country)},
                ISNULL(t.State, '') AS {nameof(CompanyInfoModel.State)},
                ISNULL(t.City, '') AS {nameof(CompanyInfoModel.City)},
                ISNULL(t.PostalCode, '') AS {nameof(CompanyInfoModel.PostalCode)},
                ISNULL(t.Address, '') AS {nameof(CompanyInfoModel.Address)},
                t.LogoUrl AS {nameof(CompanyInfoModel.LogoUrl)},
                t.SignatureUrl AS {nameof(CompanyInfoModel.SignatureUrl)},
                ISNULL(t.Website, '') AS {nameof(CompanyInfoModel.Website)}
            FROM dbo.CompanyInfos AS t
            """;

        return await connection.QueryFirstOrDefaultAsync<CompanyInfoModel>(sql);
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
