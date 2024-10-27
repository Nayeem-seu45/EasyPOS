namespace EasyPOS.Application.Features.Trades.Couriers.Commands;

public class CreateCourierCommandValidator : AbstractValidator<CreateCourierCommand>
{
    public CreateCourierCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
            //return !await _commonQuery.IsExist("dbo.Couriers", ["Name"], new { Name = name });
        //}

    }
}

