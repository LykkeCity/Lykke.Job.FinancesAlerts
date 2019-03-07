using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Job.FinancesAlerts.Client.Models;
using Refit;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// FinancesAlerts client API interface.
    /// </summary>
    [PublicAPI]
    public interface IFinancesAlertsApi
    {
        [Get("/api/alerts")]
        Task<AlertRulesData> GetAlertRulesDataAsync();

        [Post("/api/alerts/addrule")]
        Task<string> CreateAlertRuleAsync(CreateAlertRuleRequest request);

        [Put("/api/alerts/updaterule")]
        Task UpdateAlertRuleAsync(UpdateAlertRuleRequest request);

        [Delete("/api/alerts/deleterule")]
        Task DeleteAlertRuleAsync(DeleteAlertRuleRequest request);

        [Get("/api/subscriptions")]
        Task<List<AlertSubscription>> GetAlertSubscriptionsDataAsync(string alertRuleId);

        [Post("/api/alerts/addsubscription")]
        Task<string> CreateAlertSibscriptionAsync(CreateAlertSibscriptionRequest request);

        [Put("/api/alerts/updatesubscription")]
        Task UpdateAlertSubscriptionAsync(UpdateAlertSibscriptionRequest request);

        [Delete("/api/alerts/deletesubscription")]
        Task DeleteAlertSubscriptionAsync(DeleteAlertSibscriptionRequest request);
    }
}
