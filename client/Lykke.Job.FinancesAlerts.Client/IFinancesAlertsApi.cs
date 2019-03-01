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
        Task<AlertRulesData> GetAlertRulesData();

        [Post("/api/alerts/addrule")]
        Task<string> CreateAlertRule(CreateAlertRuleRequest request);

        [Put("/api/alerts/updaterule")]
        Task UpdateAlertRule(UpdateAlertRuleRequest request);

        [Delete("/api/alerts/deleterule")]
        Task DeleteAlertRule(DeleteAlertRuleRequest request);

        [Post("/api/alerts/addsubscription")]
        Task<string> CreateAlertSibscription(CreateAlertSibscriptionRequest request);

        [Put("/api/alerts/updatesubscription")]
        Task UpdateAlertSibscription(UpdateAlertSibscriptionRequest request);

        [Delete("/api/alerts/deletesubscription")]
        Task DeleteAlertSibscription(DeleteAlertSibscriptionRequest request);
    }
}
