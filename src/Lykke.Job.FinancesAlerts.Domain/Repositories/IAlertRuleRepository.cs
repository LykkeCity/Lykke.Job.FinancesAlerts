using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertRuleRepository
    {
        Task<string> AddAsync(
            string metricName,
            ComparisonType comparisonType,
            decimal threshold,
            string createdBy);

        Task UpdateAsync(
            string id,
            string metricName,
            ComparisonType comparisonType,
            decimal threshold,
            string changedBy);

        Task DeleteAsync(string metricName, string alertRuleId);

        Task<IAlertRule> GetAsync(string metricName, string id);

        Task<IEnumerable<IAlertRule>> GetByMetricAsync(string metricName);
    }
}
