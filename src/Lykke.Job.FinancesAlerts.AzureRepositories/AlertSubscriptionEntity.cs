using System;
using Lykke.Job.FinancesAlerts.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertSubscriptionEntity : TableEntity, IAlertSubscription
    {
        public string Id { get; set; }

        public string AlertRuleId { get; set; }

        [IgnoreProperty]
        public AlertSubscriptionType Type
        {
            get => Enum.Parse<AlertSubscriptionType>(TypeStr);
            set => TypeStr = value.ToString();
        }

        public string SubscriptionData { get; set; }

        public string Author { get; set; }

        public string TypeStr { get; set; }

        internal static AlertSubscriptionEntity Create(IAlertSubscription alertSubscription)
        {
            var id = Guid.NewGuid().ToString();

            return new AlertSubscriptionEntity
            {
                PartitionKey = GeneratePatitionKey(alertSubscription.AlertRuleId),
                RowKey = id,
                Id = id,
                AlertRuleId = alertSubscription.AlertRuleId,
                TypeStr = alertSubscription.Type.ToString(),
                SubscriptionData = alertSubscription.SubscriptionData,
                Author = alertSubscription.Author,
            };
        }

        internal static string GeneratePatitionKey(string alertRuleId)
        {
            return alertRuleId;
        }
    }
}
