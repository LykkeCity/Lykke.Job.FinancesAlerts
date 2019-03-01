using System;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public class AlertSubscription : IAlertSubscription
    {
        public string Id { get; set; }
        public string AlertRuleId { get; set; }
        public AlertSubscriptionType Type { get; set; }
        public string Address { get; set; }
        public TimeSpan AlertFrequency { get; set; }
        public string ChangedBy { get; set; }

        public static AlertSubscription Copy(IAlertSubscription subscription)
        {
            return new AlertSubscription
            {
                Id = subscription.Id,
                AlertRuleId = subscription.AlertRuleId,
                Type = subscription.Type,
                Address = subscription.Address,
                AlertFrequency = subscription.AlertFrequency,
                ChangedBy = subscription.ChangedBy,
            };
        }
    }
}
