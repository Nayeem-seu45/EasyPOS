﻿namespace EasyPOS.Application.Features.HRM.Employees.Commands;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
     public UpdateEmployeeCommandValidator()
     {
        RuleFor(v => v.Id).NotNull();

       //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(async (v, name, cancellation) => await BeUniqueNameSkipCurrent(name, v.Id, cancellation))
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");

        //public async Task<bool> BeUniqueNameSkipCurrent(string name, Guid id, CancellationToken cancellationToken)
        //{
        //    return !await _commonQuery.IsExist("dbo.Lookups", ["Name"], new { Name = name, Id = id }, ["Id"]);
        //}
       
     }  
}
