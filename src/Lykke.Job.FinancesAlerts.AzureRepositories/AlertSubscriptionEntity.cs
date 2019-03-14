﻿using System;
using Lykke.Job.FinancesAlerts.Client.Models;
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

        [IgnoreProperty]
        public TimeSpan AlertFrequency
        {
            get => TimeSpan.Parse(AlertFrequencyStr);
            set => AlertFrequencyStr = value.ToString();
        }

        public string Address { get; set; }

        public string ChangedBy { get; set; }

        public string TypeStr { get; set; }
        public string AlertFrequencyStr { get; set; }

        internal static AlertSubscriptionEntity Create(IAlertSubscription alertSubscription)
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
                TypeStr = alertSubscription.Type.ToString(),
                AlertFrequencyStr = alertSubscription.AlertFrequency.ToString(),
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
