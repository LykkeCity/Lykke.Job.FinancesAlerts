using FluentValidation;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Validators
{
    public class DeleteAlertRuleRequestValidator : AbstractValidator<DeleteAlertRuleRequest>
    {
        public DeleteAlertRuleRequestValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Request can't be empty");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Alert rule id can't be empty");

            RuleFor(x => x.MetricName)
                .NotEmpty()
                .WithMessage("Metric name can't be empty");

            RuleFor(x => x.ChangedBy)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAlertRuleRequest.ChangedBy)} can't be empty");
        }
    }
}
