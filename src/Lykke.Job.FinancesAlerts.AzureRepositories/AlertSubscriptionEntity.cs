using System;
using Lykke.AzureStorage.Tables;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertSubscriptionEntity : AzureTableEntity
    {
        public string Id { get; private set; }

        public string AlertRuleId { get; set; }

        public AlertSubscriptionType Type { get; set; }

        public TimeSpan AlertFrequency { get; set; }

        public string Address { get; set; }

        public string ChangedBy { get; set; }

        internal static AlertSubscriptionEntity Create(AlertSubscription alertSubscription)
        {
            var id = Guid.NewGuid().ToString();

            return new AlertSubscriptionEntity
            {
                PartitionKey = GeneratePatitionKey(alertSubscription.AlertRuleId),
                RowKey = GenerateRowKey(id),
                Id = id,
                AlertRuleId = alertSubscription.AlertRuleId,
                Address = alertSubscription.Address,
                ChangedBy = alertSubscription.ChangedBy,
                Type = alertSubscription.Type,
                AlertFrequency = alertSubscription.AlertFrequency,
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

        internal AlertSubscription ToDomain()
        {
            return new AlertSubscription
            {
                Id = Id,
                AlertRuleId = AlertRuleId,
                Type = Type,
                Address = Address,
                AlertFrequency = AlertFrequency,
                ChangedBy = ChangedBy,
            };
        }
    }
}
