using FluentValidation;
using Payments.Orchestrator.Api.Domain.ValueObjects;
using Payments.Orchestrator.Api.Application.Models;

namespace Payments.Orchestrator.Api.Application.Validators;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.MerchantId).NotEmpty().WithMessage("MerchantId is required");
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Must(Currency.IsSupported).WithMessage($"Currency must be one of: {string.Join(", ", Currency.SupportedCurrencies)}");
    }
}
