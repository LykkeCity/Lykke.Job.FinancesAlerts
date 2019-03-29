using JetBrains.Annotations;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    [PublicAPI]
    public class DeleteAlertRuleRequest
    {
        public string Id { get; set; }
        public string MetricName { get; set; }
        public string ChangedBy { get; set; }
    }
}
