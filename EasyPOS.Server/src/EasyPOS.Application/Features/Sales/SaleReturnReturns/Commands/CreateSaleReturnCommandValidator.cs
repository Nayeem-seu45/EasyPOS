namespace EasyPOS.Application.Features.SaleReturns.Commands;

public class CreateSaleReturnCommandValidator : AbstractValidator<CreateSaleReturnCommand>
{
    public CreateSaleReturnCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
        //return !await _commonQuery.IsExist("dbo.SaleReturns", ["Name"], new { Name = name });
        //}

    }
}

