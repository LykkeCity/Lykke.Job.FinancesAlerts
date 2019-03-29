using System;
using Lykke.AzureStorage.Tables;
using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertRuleEntity : AzureTableEntity, IAlertRule
    {
        public string Id { get; private set; }

        public string MetricName { get; set; }

        public string ChangedBy { get; set; }

        public ComparisonType ComparisonType { get; set; }

        public decimal ThresholdValue { get; set; }

        internal static AlertRuleEntity Create(
            string metricName,
            ComparisonType comparisonType,
            decimal threshold,
            string createdBy)
        {
            var id = Guid.NewGuid().ToString();

            return new AlertRuleEntity
            {
                PartitionKey = GeneratePatitionKey(metricName),
                RowKey = GenerateRowKey(id),
                Id = id,
                MetricName = metricName,
                ChangedBy = createdBy,
                ComparisonType = comparisonType,
                ThresholdValue = threshold,
            };
        }

        internal static string GeneratePatitionKey(string metricName)
        {
            return metricName;
        }

        internal static string GenerateRowKey(string id)
        {
            return id;
        }
    }
}
