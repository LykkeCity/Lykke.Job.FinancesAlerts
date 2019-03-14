using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.FinancesAlerts.Client.Models;
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

        public async Task<string> AddAsync(IAlertRule alertRule)
        {
            var alertRuleEntity = AlertRuleEntity.Create(alertRule);
            await _storage.InsertAsync(alertRuleEntity);
            return alertRuleEntity.Id;
        }

        public async Task UpdateAsync(IAlertRule alertRule)
        {
            await _storage.MergeAsync(
                AlertRuleEntity.GeneratePatitionKey(alertRule.MetricName),
                AlertRuleEntity.GenerateRowKey(alertRule.Id),
                i =>
                {
                    i.ComparisonType = alertRule.ComparisonType;
                    i.ThresholdValue = alertRule.ThresholdValue;
                    return i;
                });
        }

        public async Task DeleteAsync(string metricName, string alertRuleId)
        {
            await _storage.DeleteAsync(AlertRuleEntity.GeneratePatitionKey(metricName), alertRuleId);
        }

        public async Task<IAlertRule> GetAsync(string metricName, string id)
        {
            return await _storage.GetDataAsync(metricName, id);
        }

        public async Task<IEnumerable<IAlertRule>> GetByMetricAsync(string metricName)
        {
            return await _storage.GetDataAsync(metricName);
        }
    }
}
