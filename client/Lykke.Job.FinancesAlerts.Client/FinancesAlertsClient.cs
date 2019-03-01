using Lykke.HttpClientGenerator;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// FinancesAlerts API client.
    /// </summary>
    public class FinancesAlertsClient : IFinancesAlertsClient
    {
        /// <summary>Inerface to FinancesAlerts Api.</summary>
        public IFinancesAlertsApi Api { get; }

        /// <summary>C-tor</summary>
        public FinancesAlertsClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<IFinancesAlertsApi>();
        }
    }
}
