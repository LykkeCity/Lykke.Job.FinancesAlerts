using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Job.FinancesAlerts.Client.Models;
using Refit;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// Finances alerts client API interface.
    /// </summary>
    [PublicAPI]
    public interface IFinancesAlertsApi
    {
        [Get("/api/alerts")]
        Task<AlertRulesData> GetAlertRulesDataAsync();

        [Get("/api/alerts/metrics")]
        Task<List<MetricInfo>> GetAlertRulesMetricsAsync();

        [Get("/api/alerts/{alertRuleId}/metrics/{metricName}")]
        Task<AlertRule> GetAlertRuleByIdAsync(string metricName, string alertRuleId);

        [Post("/api/alerts")]
        Task<string> CreateAlertRuleAsync(CreateAlertRuleRequest request);

        [Put("/api/alerts")]
        Task UpdateAlertRuleAsync(UpdateAlertRuleRequest request);

        [Delete("/api/alerts")]
        Task DeleteAlertRuleAsync(DeleteAlertRuleRequest request);
    }
}
