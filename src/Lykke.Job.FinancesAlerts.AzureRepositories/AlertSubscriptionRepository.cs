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

        public async Task AddAsync(IAlertSubscription alertSubscription)
        {
            await _storage.InsertAsync(AlertSubscriptionEntity.Create(alertSubscription)).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string alertRuleId, string alertSubscriptionId)
        {
            await _storage.DeleteAsync(AlertSubscriptionEntity.GeneratePatitionKey(alertRuleId), alertSubscriptionId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IAlertSubscription>> GetByAlertRuleAsync(string alertRuleId)
        {
            var result = await _storage.GetDataAsync(alertRuleId).ConfigureAwait(false);
            return result ?? new List<AlertSubscriptionEntity>();
        }
    }
}
