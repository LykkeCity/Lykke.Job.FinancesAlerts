using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertSubscriptionRepository
    {
        Task<string> AddAsync(AlertSubscription alertSubscription);

        Task UpdateAsync(AlertSubscription alertSubscription);

        Task DeleteAsync(string alertRuleId, string alertSubscriptionId);

        Task<AlertSubscription> GetAsync(string alertRuleId, string id);

        Task<IEnumerable<AlertSubscription>> GetByAlertRuleAsync(string alertRuleId);
    }
}
