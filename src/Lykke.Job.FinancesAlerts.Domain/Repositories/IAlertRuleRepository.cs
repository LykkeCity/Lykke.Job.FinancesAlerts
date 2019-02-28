using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertRuleRepository
    {
        Task AddAsync(IAlertRule alertRule);

        Task RemoveAsync(string metricName, string alertRuleId);

        Task<IEnumerable<IAlertRule>> GetByMetricAsync(string metricName);
    }
}
