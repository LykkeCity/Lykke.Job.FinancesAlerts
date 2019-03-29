using System;

namespace Lykke.Job.FinancesAlerts.Domain
{
    public interface IAlertSubscription
    {
        string Id { get; }
        string AlertRuleId { get; set; }
        AlertSubscriptionType Type { get; set; }
        string Address { get; set; }
        TimeSpan AlertFrequency { get; set; }
        string ChangedBy { get; set; }
    }
}
