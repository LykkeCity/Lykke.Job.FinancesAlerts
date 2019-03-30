using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class AlertSubscriptionEntity : AzureTableEntity, IAlertSubscription
    {
        public string Id { get; private set; }

        public string AlertRuleId { get; set; }

        private AlertSubscriptionType _type;
        public AlertSubscriptionType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        private TimeSpan _alertFrequency;
        public TimeSpan AlertFrequency
        {
            get => _alertFrequency;
            set
            {
                if (_alertFrequency != value)
                {
                    _alertFrequency = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
        
        public string Address { get; set; }

        public string ChangedBy { get; set; }

        internal static AlertSubscriptionEntity Create(
            string alertRuleId,
            AlertSubscriptionType subscriptionType,
            string address,
            TimeSpan alertFrequency,
            string createdBy)
        {
            var id = Guid.NewGuid().ToString();

            return new AlertSubscriptionEntity
            {
                PartitionKey = GeneratePatitionKey(alertRuleId),
                RowKey = GenerateRowKey(id),
                Id = id,
                AlertRuleId = alertRuleId,
                Address = address,
                Type = subscriptionType,
                AlertFrequency = alertFrequency,
                ChangedBy = createdBy,
            };
        }

        internal static string GeneratePatitionKey(string alertRuleId)
        {
            return alertRuleId;
        }

        internal static string GenerateRowKey(string id)
        {
            return id;
        }
    }
}
