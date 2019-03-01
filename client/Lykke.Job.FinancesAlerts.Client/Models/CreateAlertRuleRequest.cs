namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public class CreateAlertRuleRequest
    {
        public string MetricName { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public decimal ThresholdValue { get; set; }
        public string ChangedBy { get; set; }
    }
}
