using JetBrains.Annotations;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// FinancesAlerts client interface.
    /// </summary>
    [PublicAPI]
    public interface IFinancesAlertsClient
    {
        /// <summary>Application Api interface</summary>
        IFinancesAlertsApi Api { get; }
    }
}
