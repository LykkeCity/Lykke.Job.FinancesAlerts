using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertRuleRepository
    {
        Task<string> AddAsync(IAlertRule alertRule);

        Task UpdateAsync(IAlertRule alertRule);

        Task DeleteAsync(string metricName, string alertRuleId);

        Task<IAlertRule> GetAsync(string metricName, string id);

        Task<IEnumerable<IAlertRule>> GetByMetricAsync(string metricName);
    }
}
