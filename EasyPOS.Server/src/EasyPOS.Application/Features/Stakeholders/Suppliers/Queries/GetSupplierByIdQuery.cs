﻿using EasyPOS.Application.Features.Suppliers.Models;

namespace EasyPOS.Application.Features.Suppliers.Queries;

public record GetSupplierByIdQuery(Guid Id) : ICacheableQuery<SupplierModel>
{
    public string CacheKey => $"{CacheKeys.Supplier}_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => true;
}

internal sealed class GetSupplierByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetSupplierByIdQuery, SupplierModel>
{
    public async Task<Result<SupplierModel>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new SupplierModel();
        }
        var connection = sqlConnectionFactory.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(SupplierModel.Id)},
                t.Name AS {nameof(SupplierModel.Name)},
                t.Email AS {nameof(SupplierModel.Email)},
                t.PhoneNo AS {nameof(SupplierModel.PhoneNo)},
                t.Mobile AS {nameof(SupplierModel.Mobile)},
                t.Country AS {nameof(SupplierModel.Country)},
                t.City AS {nameof(SupplierModel.City)},
                t.Address AS {nameof(SupplierModel.Address)},
                t.IsActive AS {nameof(SupplierModel.IsActive)}
            FROM dbo.Suppliers t
            WHERE t.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<SupplierModel>(sql, new { request.Id });
    }
}
