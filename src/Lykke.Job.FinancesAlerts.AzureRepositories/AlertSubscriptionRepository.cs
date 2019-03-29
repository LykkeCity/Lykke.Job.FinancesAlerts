using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.FinancesAlerts.Client.Models;
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

        public async Task<string> AddAsync(AlertSubscription alertSubscription)
        {
            var alertSubscriptionEntity = AlertSubscriptionEntity.Create(alertSubscription);
            await _storage.InsertAsync(alertSubscriptionEntity);
            return alertSubscriptionEntity.Id;
        }

        public async Task UpdateAsync(AlertSubscription alertSubscription)
        {
            await _storage.MergeAsync(
                AlertSubscriptionEntity.GeneratePatitionKey(alertSubscription.AlertRuleId),
                AlertSubscriptionEntity.GenerateRowKey(alertSubscription.Id),
                i =>
                {
                    i.AlertFrequency = alertSubscription.AlertFrequency;
                    i.Type = alertSubscription.Type;
                    i.Address = alertSubscription.Address;
                    return i;
                });
        }

        public async Task DeleteAsync(string alertRuleId, string alertSubscriptionId)
        {
            await _storage.DeleteAsync(AlertSubscriptionEntity.GeneratePatitionKey(alertRuleId), alertSubscriptionId);
        }

        public async Task<AlertSubscription> GetAsync(string alertRuleId, string id)
        {
            var item = await _storage.GetDataAsync(alertRuleId, id);
            return item?.ToDomain();
        }

        public async Task<IEnumerable<AlertSubscription>> GetByAlertRuleAsync(string alertRuleId)
        {
            var items = await _storage.GetDataAsync(alertRuleId);
            return items.Select(i => i.ToDomain());
        }
    }
}
