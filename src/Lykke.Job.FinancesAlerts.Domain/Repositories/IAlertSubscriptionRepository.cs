using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertSubscriptionRepository
    {
        Task<string> AddAsync(IAlertSubscription alertSubscription);

        Task UpdateAsync(IAlertSubscription alertSubscription);

        Task DeleteAsync(string alertRuleId, string alertSubscriptionId);

        Task<IAlertSubscription> GetAsync(string alertRuleId, string id);

        Task<IEnumerable<IAlertSubscription>> GetByAlertRuleAsync(string alertRuleId);
    }
}
