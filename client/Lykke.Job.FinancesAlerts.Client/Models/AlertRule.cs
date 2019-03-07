namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public class AlertRule : IAlertRule
    {
        public string Id { get; set; }
        public string MetricName { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public decimal ThresholdValue { get; set; }
        public string ChangedBy { get; set; }

        public int SubscriptionsCount { get; set; }

        public static AlertRule Copy(IAlertRule alertRule)
        {
            return new AlertRule
            {
                Id = alertRule.Id,
                MetricName = alertRule.MetricName,
                ComparisonType = alertRule.ComparisonType,
                ThresholdValue = alertRule.ThresholdValue,
                ChangedBy = alertRule.ChangedBy,
            };
        }
    }
}
