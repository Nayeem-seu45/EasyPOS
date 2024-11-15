namespace EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Commands;

public class CreatePurchaseReturnPaymentCommandValidator : AbstractValidator<CreatePurchaseReturnPaymentCommand>
{
    public CreatePurchaseReturnPaymentCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
        //return !await _commonQuery.IsExist("dbo.PurchasePayments", ["Name"], new { Name = name });
        //}

    }
}

