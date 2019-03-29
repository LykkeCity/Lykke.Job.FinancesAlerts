using JetBrains.Annotations;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    [PublicAPI]
    public class AlertRule
    {
        public string Id { get; set; }
        public string MetricName { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public decimal ThresholdValue { get; set; }
        public string ChangedBy { get; set; }
        public int SubscriptionsCount { get; set; }
    }
}
