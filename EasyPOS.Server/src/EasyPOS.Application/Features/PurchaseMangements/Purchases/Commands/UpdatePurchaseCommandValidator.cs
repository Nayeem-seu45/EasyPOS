﻿namespace EasyPOS.Application.Features.Purchases.Commands;

public class UpdatPurchaseCommandValidator : AbstractValidator<UpdatePurchaseCommand>
{
    private readonly ICommonQueryService _commonQuery;

    public UpdatPurchaseCommandValidator(ICommonQueryService commonQuery)
    {
        _commonQuery = commonQuery;

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(async (v, name, cancellation) => await BeUniqueNameSkipCurrent(name, v.Id, cancellation))
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");

        //RuleFor(v => v.Address)
        //    .MaximumLength(5000)
        //    .WithMessage("{0} can not exceed max {1} chars.");

    }

    public async Task<bool> BeUniqueNameSkipCurrent(string name, Guid id, CancellationToken cancellationToken)
    {
        return !await _commonQuery.IsExistAsync("dbo.Purchases", ["Name"], new { Name = name, Id = id }, ["Id"]);
    }
}
