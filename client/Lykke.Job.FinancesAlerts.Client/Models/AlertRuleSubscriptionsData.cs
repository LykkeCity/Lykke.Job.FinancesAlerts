using System.Collections.Generic;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public class AlertRuleSubscriptionsData
    {
        public string AlertRuleId { get; set; }

        public List<AlertSubscription> Subscriptions { get; set; }

        public List<AlertSubscriptionType> SubscriptionTypes { get; set; }
    }
}
