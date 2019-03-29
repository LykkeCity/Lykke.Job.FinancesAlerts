using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertRuleRepository
    {
        Task<string> AddAsync(AlertRule alertRule);

        Task UpdateAsync(AlertRule alertRule);

        Task DeleteAsync(string metricName, string alertRuleId);

        Task<AlertRule> GetAsync(string metricName, string id);

        Task<IEnumerable<AlertRule>> GetByMetricAsync(string metricName);
    }
}
