using FluentValidation;
using Payments.Orchestrator.Api.Domain;
using Payments.Orchestrator.Api.Models;

namespace Payments.Orchestrator.Api.Validators;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Must(Currency.IsSupported).WithMessage($"Currency must be one of: {string.Join(", ", Currency.SupportedCurrencies)}");
    }
}
