using System;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public interface IAlertSubscription
    {
        string Id { get; set; }

        string AlertRuleId { get; set; }

        AlertSubscriptionType Type { get; set; }

        string Address { get; set; }

        TimeSpan AlertFrequency { get; set; }

        string ChangedBy { get; set; }
    }
}
