using System.Collections.Generic;
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

        public async Task<string> AddAsync(
            string metricName,
            ComparisonType comparisonType,
            decimal threshold,
            string createdBy)
        {
            var alertRuleEntity = AlertRuleEntity.Create(
                metricName,
                comparisonType,
                threshold,
                createdBy);
            await _storage.InsertAsync(alertRuleEntity);
            return alertRuleEntity.Id;
        }

        public async Task UpdateAsync(
            string id,
            string metricName,
            ComparisonType comparisonType,
            decimal threshold,
            string changedBy)
        {
            await _storage.MergeAsync(
                AlertRuleEntity.GeneratePatitionKey(metricName),
                AlertRuleEntity.GenerateRowKey(id),
                i =>
                {
                    i.ComparisonType = comparisonType;
                    i.ThresholdValue = threshold;
                    i.ChangedBy = changedBy;
                    return i;
                });
        }

        public Task DeleteAsync(string metricName, string alertRuleId)
        {
            return _storage.DeleteAsync(AlertRuleEntity.GeneratePatitionKey(metricName), alertRuleId);
        }

        public async Task<IAlertRule> GetAsync(string metricName, string id)
        {
            var result = await _storage.GetDataAsync(metricName, id);
            return result;
        }

        public async Task<IEnumerable<IAlertRule>> GetByMetricAsync(string metricName)
        {
            var result = await _storage.GetDataAsync(metricName);
            return result;
        }
    }
}
