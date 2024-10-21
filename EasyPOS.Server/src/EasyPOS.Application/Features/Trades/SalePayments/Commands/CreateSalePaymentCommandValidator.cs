namespace EasyPOS.Application.Features.Trades.SalePayments.Commands;

public class CreateSalePaymentCommandValidator : AbstractValidator<CreateSalePaymentCommand>
{
    public CreateSalePaymentCommandValidator()
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

