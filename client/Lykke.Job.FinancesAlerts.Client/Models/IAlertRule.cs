namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public interface IAlertRule
    {
        string Id { get; set; }

        string MetricName { get; set; }

        ComparisonType ComparisonType { get; set; }

        decimal ThresholdValue { get; set; }

        string ChangedBy { get; set; }
    }
}
