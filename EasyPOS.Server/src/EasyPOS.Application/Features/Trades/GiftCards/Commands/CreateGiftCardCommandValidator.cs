﻿namespace EasyPOS.Application.Features.Trades.GiftCards.Commands;

public class CreateGiftCardCommandValidator : AbstractValidator<CreateGiftCardCommand>
{
    public CreateGiftCardCommandValidator()
    {

        //RuleFor(v => v.Name)
        //    .NotEmpty()
        //    .MaximumLength(200)
        //    .MustAsync(BeUniqueName)
        //        .WithMessage("'{PropertyName}' must be unique.")
        //        .WithErrorCode("Unique");
        //public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        //{
            //return !await _commonQuery.IsExist("dbo.GiftCards", ["Name"], new { Name = name });
        //}

    }
}

