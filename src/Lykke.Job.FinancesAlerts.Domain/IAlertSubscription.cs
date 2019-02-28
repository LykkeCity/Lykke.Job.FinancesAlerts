using System;

namespace Lykke.Job.FinancesAlerts.Domain
{
    public interface IAlertSubscription
    {
        string Id { get; set; }

        string AlertRuleId { get; set; }

        AlertSubscriptionType Type { get; set; }

        string SubscriptionData { get; set; }

        TimeSpan AlertFrequency { get; set; }

        string Author { get; set; }
    }
}
