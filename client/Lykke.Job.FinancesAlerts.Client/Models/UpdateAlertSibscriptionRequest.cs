using System;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public class UpdateAlertSibscriptionRequest
    {
        public string Id { get; set; }
        public string AlertRuleId { get; set; }
        public AlertSubscriptionType Type { get; set; }
        public string Address { get; set; }
        public TimeSpan AlertFrequency { get; set; }
        public string ChangedBy { get; set; }
    }
}
