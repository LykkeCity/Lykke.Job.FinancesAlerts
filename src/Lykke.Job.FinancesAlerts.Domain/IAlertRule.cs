namespace Lykke.Job.FinancesAlerts.Domain
{
    public interface IAlertRule
    {
        string Id { get; }
        string MetricName { get; set; }
        ComparisonType ComparisonType { get; set; }
        decimal ThresholdValue { get; set; }
        string ChangedBy { get; set; }
    }
}
