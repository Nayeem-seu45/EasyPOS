namespace EasyPOS.Application.Features.Quotations.Commands;

public class CreateQuotationCommandValidator : AbstractValidator<CreateQuotationCommand>
{
    public CreateQuotationCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
        //return !await _commonQuery.IsExist("dbo.Quotations", ["Name"], new { Name = name });
        //}

    }
}

