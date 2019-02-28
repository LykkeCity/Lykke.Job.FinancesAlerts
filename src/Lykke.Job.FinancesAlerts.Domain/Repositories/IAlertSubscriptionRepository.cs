using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertSubscriptionRepository
    {
        Task AddAsync(IAlertSubscription alertSubscription);

        Task RemoveAsync(string alertRuleId, string alertSubscriptionId);

        Task<IEnumerable<IAlertSubscription>> GetByAlertRuleAsync(string alertRuleId);
    }
}
