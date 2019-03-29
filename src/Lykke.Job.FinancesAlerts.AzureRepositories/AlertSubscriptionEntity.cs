using System;
using Lykke.AzureStorage.Tables;
using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertSubscriptionEntity : AzureTableEntity, IAlertSubscription
    {
        public string Id { get; private set; }

        public string AlertRuleId { get; set; }

        public AlertSubscriptionType Type { get; set; }

        public TimeSpan AlertFrequency { get; set; }

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
