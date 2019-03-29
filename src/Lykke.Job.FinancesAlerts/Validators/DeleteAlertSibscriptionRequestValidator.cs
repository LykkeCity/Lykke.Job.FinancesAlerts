using FluentValidation;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Validators
{
    public class DeleteAlertSibscriptionRequestValidator : AbstractValidator<DeleteAlertSibscriptionRequest>
    {
        public DeleteAlertSibscriptionRequestValidator()
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

            RuleFor(x => x.ChangedBy)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAlertRuleRequest.ChangedBy)} can't be empty");
        }
    }
}
