using System;
using System.Globalization;
using Lykke.Job.FinancesAlerts.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertRuleEntity : TableEntity, IAlertRule
    {
        public string MetricName { get; set; }

        public string Author { get; set; }

        [IgnoreProperty]
        public ComparisonType ComparisonType
        {
            get => Enum.Parse<ComparisonType>(ComparisonTypeStr);
            set => ComparisonTypeStr = value.ToString();
        }

        public string ComparisonTypeStr { get; set; }

        [IgnoreProperty]
        public decimal ThresholdValue
        {
            get => decimal.Parse(ThresholdValueStr);
            set => ThresholdValueStr = value.ToString(CultureInfo.InvariantCulture);
        }

        public string ThresholdValueStr { get; set; }

        internal static AlertRuleEntity Create(IAlertRule alertRule)
        {
            return new AlertRuleEntity
            {
                PartitionKey = GeneratePatitionKey(),
                RowKey = Guid.NewGuid().ToString(),
                MetricName = alertRule.MetricName,
                ComparisonTypeStr = alertRule.ComparisonType.ToString(),
                ThresholdValueStr = alertRule.ThresholdValue.ToString(CultureInfo.InvariantCulture),
            };
        }

        internal static string GeneratePatitionKey()
        {
            return "AlertRule";
        }
    }
}
