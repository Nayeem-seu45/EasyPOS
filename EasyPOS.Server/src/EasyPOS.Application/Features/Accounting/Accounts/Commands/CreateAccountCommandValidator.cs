﻿namespace EasyPOS.Application.Features.Accounting.Accounts.Commands;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {

        RuleFor(v => v.Name)
             .MaximumLength(256)
             .NotEmpty();

    }
}

