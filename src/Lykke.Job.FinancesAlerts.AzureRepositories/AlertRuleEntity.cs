using System;
using System.Globalization;
using Lykke.Job.FinancesAlerts.Client.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertRuleEntity : TableEntity, IAlertRule
    {
        public string Id { get; set; }

        public string MetricName { get; set; }

        public string ChangedBy { get; set; }

        [IgnoreProperty]
        public ComparisonType ComparisonType
        {
            get => Enum.Parse<ComparisonType>(ComparisonTypeStr);
            set => ComparisonTypeStr = value.ToString();
        }

        [IgnoreProperty]
        public decimal ThresholdValue
        {
            get => decimal.Parse(ThresholdValueStr);
            set => ThresholdValueStr = value.ToString(CultureInfo.InvariantCulture);
        }

        public string ThresholdValueStr { get; set; }
        public string ComparisonTypeStr { get; set; }

        internal static AlertRuleEntity Create(IAlertRule alertRule)
        {
            var id = Guid.NewGuid().ToString();

            return new AlertRuleEntity
            {
                PartitionKey = GeneratePatitionKey(alertRule.MetricName),
                RowKey = GenerateRowKey(id),
                Id = id,
                MetricName = alertRule.MetricName,
                ChangedBy = alertRule.ChangedBy,
                ComparisonTypeStr = alertRule.ComparisonType.ToString(),
                ThresholdValueStr = alertRule.ThresholdValue.ToString(CultureInfo.InvariantCulture),
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
