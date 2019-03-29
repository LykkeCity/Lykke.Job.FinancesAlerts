using System;
using Lykke.AzureStorage.Tables;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertRuleEntity : AzureTableEntity
    {
        public string Id { get; private set; }

        public string MetricName { get; set; }

        public string ChangedBy { get; set; }

        public ComparisonType ComparisonType { get; set; }

        public decimal ThresholdValue { get; set; }

        internal static AlertRuleEntity Create(AlertRule alertRule)
        {
            var id = Guid.NewGuid().ToString();

            return new AlertRuleEntity
            {
                PartitionKey = GeneratePatitionKey(alertRule.MetricName),
                RowKey = GenerateRowKey(id),
                Id = id,
                MetricName = alertRule.MetricName,
                ChangedBy = alertRule.ChangedBy,
                ComparisonType = alertRule.ComparisonType,
                ThresholdValue = alertRule.ThresholdValue,
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

        internal AlertRule ToDomain()
        {
            return new AlertRule
            {
                Id = Id,
                MetricName = MetricName,
                ComparisonType = ComparisonType,
                ThresholdValue = ThresholdValue,
                ChangedBy = ChangedBy,
            };
        }
    }
}
