﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.FinancesAlerts.Domain;
using Lykke.Job.FinancesAlerts.Domain.Repositories;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class AlertRuleRepository : IAlertRuleRepository
    {
        private readonly INoSQLTableStorage<AlertRuleEntity> _storage;

        public AlertRuleRepository(INoSQLTableStorage<AlertRuleEntity> storage)
        {
            _storage = storage;
        }

        public async Task AddAsync(IAlertRule alertRule)
        {
            await _storage.InsertAsync(AlertRuleEntity.Create(alertRule)).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string metricName, string alertRuleId)
        {
            await _storage.DeleteAsync(AlertRuleEntity.GeneratePatitionKey(metricName), alertRuleId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IAlertRule>> GetByMetricAsync(string metricName)
        {
            var result = await _storage.GetDataAsync(metricName).ConfigureAwait(false);
            return result ?? new List<AlertRuleEntity>();
        }
    }
}
