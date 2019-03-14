using JetBrains.Annotations;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// FinancesAlerts client interface.
    /// </summary>
    [PublicAPI]
    public interface IFinancesAlertsClient
    {
        /// <summary>Alerts Api interface</summary>
        IFinancesAlertsApi AlertsApi { get; }

        /// <summary>Application Api interface</summary>
        IFinancesAlertSubscriptionsApi AlertSubscriptionsApi { get; }
    }
}
