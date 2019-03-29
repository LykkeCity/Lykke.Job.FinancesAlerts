using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Job.FinancesAlerts.Client.Models;
using Refit;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// Finances alert subscriptions client API interface.
    /// </summary>
    [PublicAPI]
    public interface IFinancesAlertSubscriptionsApi
    {
        [Get("/api/subscriptions")]
        Task<List<AlertSubscription>> GetAlertSubscriptionsDataAsync(string alertRuleId);

        [Get("/api/subscriptions/{alertSubscriptionId}/rules/{alertRuleId}")]
        Task<AlertSubscription> GetAlertSubscriptionByIdAsync(string alertRuleId, string alertSubscriptionId);

        [Post("/api/subscriptions")]
        Task<string> CreateAlertSibscriptionAsync(CreateAlertSibscriptionRequest request);

        [Put("/api/subscriptions")]
        Task UpdateAlertSubscriptionAsync(UpdateAlertSibscriptionRequest request);

        [Delete("/api/subscriptions")]
        Task DeleteAlertSubscriptionAsync(DeleteAlertSibscriptionRequest request);
    }
}
