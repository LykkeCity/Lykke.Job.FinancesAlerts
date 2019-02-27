using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertRuleRepository
    {
        Task AddAlertRuleAsync(IAlertRule alertRule);

        Task RemoveAlertRuleAsync(string alertRuleId);

        Task<IEnumerable<IAlertRule>> GetMetricRulesAsync(string metricName);
    }
}
