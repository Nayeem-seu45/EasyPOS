namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Commands;

public class CreateProductAdjustmentCommandValidator : AbstractValidator<CreateProductAdjustmentCommand>
{
    public CreateProductAdjustmentCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
            //return !await _commonQuery.IsExist("dbo.ProductAdjustments", ["Name"], new { Name = name });
        //}

    }
}

