using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface IAlertSubscriptionRepository
    {
        Task<string> AddAsync(
            string alertRuleId,
            AlertSubscriptionType subscriptionType,
            string address,
            TimeSpan alertFrequency,
            string createdBy);

        Task UpdateAsync(
            string id,
            string alertRuleId,
            AlertSubscriptionType subscriptionType,
            string address,
            TimeSpan alertFrequency,
            string changedBy);

        Task DeleteAsync(string alertRuleId, string alertSubscriptionId);

        Task<IAlertSubscription> GetAsync(string alertRuleId, string id);

        Task<IEnumerable<IAlertSubscription>> GetByAlertRuleAsync(string alertRuleId);
    }
}
