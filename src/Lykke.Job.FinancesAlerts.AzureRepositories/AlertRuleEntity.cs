using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class AlertRuleEntity : AzureTableEntity, IAlertRule
    {
        public string Id { get; private set; }

        public string MetricName { get; set; }

        public string ChangedBy { get; set; }

        private ComparisonType _comparisonType;
        public ComparisonType ComparisonType
        {
            get => _comparisonType;
            set
            {
                if (_comparisonType != value)
                {
                    _comparisonType = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        private decimal _thresholdValue;
        public decimal ThresholdValue
        {
            get => _thresholdValue;
            set
            {
                if (_thresholdValue != value)
                {
                    _thresholdValue = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

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
