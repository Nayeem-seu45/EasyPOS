namespace EasyPOS.Application.Features.ProductManagement.CountStocks.Commands;

public class CreateCountStockCommandValidator : AbstractValidator<CreateCountStockCommand>
{
    public CreateCountStockCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
            //return !await _commonQuery.IsExist("dbo.CountStocks", ["Name"], new { Name = name });
        //}

    }
}

