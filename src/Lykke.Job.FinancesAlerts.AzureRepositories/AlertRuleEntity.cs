using System;
using System.Globalization;
using Lykke.Job.FinancesAlerts.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertRuleEntity : TableEntity, IAlertRule
    {
        public string Id { get; set; }

        public string MetricName { get; set; }

        public string Author { get; set; }

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
                RowKey = id,
                Id = id,
                MetricName = alertRule.MetricName,
                ComparisonTypeStr = alertRule.ComparisonType.ToString(),
                ThresholdValueStr = alertRule.ThresholdValue.ToString(CultureInfo.InvariantCulture),
            };
        }

        internal static string GeneratePatitionKey(string metricName)
        {
            return metricName;
        }
    }
}
