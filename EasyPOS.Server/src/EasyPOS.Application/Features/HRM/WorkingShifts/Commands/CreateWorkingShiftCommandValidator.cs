namespace EasyPOS.Application.Features.HRM.WorkingShifts.Commands;

public class CreateWorkingShiftCommandValidator : AbstractValidator<CreateWorkingShiftCommand>
{
    public CreateWorkingShiftCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
            //return !await _commonQuery.IsExist("dbo.WorkingShifts", ["Name"], new { Name = name });
        //}

    }
}

