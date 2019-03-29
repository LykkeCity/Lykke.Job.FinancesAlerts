using JetBrains.Annotations;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    [PublicAPI]
    public class UpdateAlertRuleRequest
    {
        public string Id { get; set; }
        public string MetricName { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public decimal ThresholdValue { get; set; }
        public string ChangedBy { get; set; }
    }
}
