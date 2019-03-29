using System;
using FluentValidation;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Validators
{
    public class UpdateAlertSibscriptionRequestValidator : AbstractValidator<UpdateAlertSibscriptionRequest>
    {
        public UpdateAlertSibscriptionRequestValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Request can't be empty");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Alert subscription id can't be empty");

            RuleFor(x => x.AlertRuleId)
                .NotEmpty()
                .WithMessage("Alert rule id can't be empty");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Subscription address can't be empty");

            RuleFor(x => x.AlertFrequency)
                .Must(i => i > TimeSpan.Zero)
                .WithMessage("Alert frequency must be positive");

            RuleFor(x => x.ChangedBy)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAlertRuleRequest.ChangedBy)} can't be empty");
        }
    }
}
