using Lykke.Job.FinancesAlerts.Client.Models;
using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.Extensions
{
    internal static class AlertSubscriptionMapExtensions
    {
        internal static AlertSubscription ToClient(this IAlertSubscription alertSubscription)
        {
            return new AlertSubscription
            {
                Id = alertSubscription.Id,
                AlertRuleId = alertSubscription.AlertRuleId,
                Type = alertSubscription.Type.ToClient(),
                Address = alertSubscription.Address,
                AlertFrequency = alertSubscription.AlertFrequency,
                ChangedBy = alertSubscription.ChangedBy,
            };
        }
    }
}
