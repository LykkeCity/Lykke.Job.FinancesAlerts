using Common;
using FluentValidation;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Validators
{
    public class CreateAlertRuleRequestValidator : AbstractValidator<CreateAlertRuleRequest>
    {
        public CreateAlertRuleRequestValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Request can't be empty");

            RuleFor(x => x.MetricName)
                .NotEmpty()
                .Must(x => x.IsValidPartitionOrRowKey())
                .WithMessage("Metric name can't be empty and must be a valid azure key");

            RuleFor(x => x.ChangedBy)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAlertRuleRequest.ChangedBy)} can't be empty");
        }
    }
}
