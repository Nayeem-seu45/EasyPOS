namespace EasyPOS.Application.Features.ProductTransfers.Commands;

public class CreateProductTransferCommandValidator : AbstractValidator<CreateProductTransferCommand>
{
    private readonly ICommonQueryService _commonQuery;

    public CreateProductTransferCommandValidator(ICommonQueryService commonQuery)
    {
        _commonQuery = commonQuery;

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(250)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");

        //RuleFor(v => v.Address)
        //    .MaximumLength(500)
        //    .WithMessage("{0} can not exceed max {1} chars.");
    }

    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _commonQuery.IsExistAsync("dbo.ProductTransfers", ["Name"], new { Name = name });
    }
}
