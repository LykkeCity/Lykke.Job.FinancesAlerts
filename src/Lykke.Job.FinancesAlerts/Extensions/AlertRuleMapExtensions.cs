using Lykke.Job.FinancesAlerts.Client.Models;
using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.Extensions
{
    internal static class AlertRuleMapExtensions
    {
        internal static AlertRule ToClient(this IAlertRule alertRule, int subscriptionsCount)
        {
            return new AlertRule
            {
                Id = alertRule.Id,
                MetricName = alertRule.MetricName,
                ComparisonType = alertRule.ComparisonType.ToClient(),
                ThresholdValue = alertRule.ThresholdValue,
                ChangedBy = alertRule.ChangedBy,
                SubscriptionsCount = subscriptionsCount,
            };
        }
    }
}
