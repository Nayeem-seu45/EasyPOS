using EasyPOS.Application.Features.Sales.SaleReturnPayments.Commands;

namespace EasyPOS.Application.Features.Sales.SalePayments.Commands;

public class CreateSaleReturnPaymentCommandValidator : AbstractValidator<CreateSaleReturnPaymentCommand>
{
    public CreateSaleReturnPaymentCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
        //return !await _commonQuery.IsExist("dbo.SalePayments", ["Name"], new { Name = name });
        //}

    }
}

