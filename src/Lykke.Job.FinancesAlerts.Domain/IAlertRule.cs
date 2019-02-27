namespace Lykke.Job.FinancesAlerts.Domain
{
    public interface IAlertRule
    {
        string MetricName { get; set; }

        ComparisonType ComparisonType { get; set; }

        decimal ThresholdValue { get; set; }

        string Author { get; set; }
    }
}
