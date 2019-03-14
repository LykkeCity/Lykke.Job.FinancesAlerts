using System.Collections.Generic;
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

        public async Task<string> AddAsync(IAlertSubscription alertSubscription)
        {
            var alertSubscriptionEntity = AlertSubscriptionEntity.Create(alertSubscription);
            await _storage.InsertAsync(alertSubscriptionEntity);
            return alertSubscriptionEntity.Id;
        }

        public async Task UpdateAsync(IAlertSubscription alertSubscription)
        {
            await _storage.ReplaceAsync(AlertSubscriptionEntity.Create(alertSubscription));
        }

        public async Task DeleteAsync(string alertRuleId, string alertSubscriptionId)
        {
            await _storage.DeleteAsync(AlertSubscriptionEntity.GeneratePatitionKey(alertRuleId), alertSubscriptionId);
        }

        public async Task<IAlertSubscription> GetAsync(string alertRuleId, string id)
        {
            return await _storage.GetDataAsync(alertRuleId, id);
        }

        public async Task<IEnumerable<IAlertSubscription>> GetByAlertRuleAsync(string alertRuleId)
        {
            return await _storage.GetDataAsync(alertRuleId);
        }
    }
}
