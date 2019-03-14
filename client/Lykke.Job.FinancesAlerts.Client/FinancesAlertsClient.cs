using Lykke.HttpClientGenerator;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// FinancesAlerts API client.
    /// </summary>
    public class FinancesAlertsClient : IFinancesAlertsClient
    {
        /// <summary>Inerface to finances alerts api.</summary>
        public IFinancesAlertsApi AlertsApi { get; }

        /// <summary>Inerface to finances alert subscriptions api.</summary>
        public IFinancesAlertSubscriptionsApi AlertSubscriptionsApi { get; }

        /// <summary>C-tor</summary>
        public FinancesAlertsClient(IHttpClientGenerator httpClientGenerator)
        {
            AlertsApi = httpClientGenerator.Generate<IFinancesAlertsApi>();
            AlertSubscriptionsApi = httpClientGenerator.Generate<IFinancesAlertSubscriptionsApi>();
        }
    }
}
