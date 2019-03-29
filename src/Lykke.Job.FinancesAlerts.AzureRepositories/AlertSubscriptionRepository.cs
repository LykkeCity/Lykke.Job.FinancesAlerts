using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.FinancesAlerts.Domain;
using Lykke.Job.FinancesAlerts.Domain.Repositories;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertSubscriptionRepository : IAlertSubscriptionRepository
    {
        private readonly INoSQLTableStorage<AlertSubscriptionEntity> _storage;

        public AlertSubscriptionRepository(INoSQLTableStorage<AlertSubscriptionEntity> storage)
        {
            _storage = storage;
        }

        public async Task<string> AddAsync(
            string alertRuleId,
            AlertSubscriptionType subscriptionType,
            string address,
            TimeSpan alertFrequency,
            string createdBy)
        {
            var alertSubscriptionEntity = AlertSubscriptionEntity.Create(
                alertRuleId,
                subscriptionType,
                address,
                alertFrequency,
                createdBy);
            await _storage.InsertAsync(alertSubscriptionEntity);
            return alertSubscriptionEntity.Id;
        }

        public async Task UpdateAsync(
            string id,
            string alertRuleId,
            AlertSubscriptionType subscriptionType,
            string address,
            TimeSpan alertFrequency,
            string changedBy)
        {
            await _storage.MergeAsync(
                AlertSubscriptionEntity.GeneratePatitionKey(alertRuleId),
                AlertSubscriptionEntity.GenerateRowKey(id),
                i =>
                {
                    i.AlertFrequency = alertFrequency;
                    i.Type = subscriptionType;
                    i.Address = address;
                    i.ChangedBy = changedBy;
                    return i;
                });
        }

        public async Task DeleteAsync(string alertRuleId, string alertSubscriptionId)
        {
            await _storage.DeleteAsync(AlertSubscriptionEntity.GeneratePatitionKey(alertRuleId), alertSubscriptionId);
        }

        public async Task<IAlertSubscription> GetAsync(string alertRuleId, string id)
        {
            var item = await _storage.GetDataAsync(alertRuleId, id);
            return item;
        }

        public async Task<IEnumerable<IAlertSubscription>> GetByAlertRuleAsync(string alertRuleId)
        {
            var items = await _storage.GetDataAsync(alertRuleId);
            return items;
        }
    }
}
