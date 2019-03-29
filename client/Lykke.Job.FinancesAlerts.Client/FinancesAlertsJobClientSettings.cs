using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// FinancesAlerts client settings.
    /// </summary>
    public class FinancesAlertsJobClientSettings
    {
        /// <summary>Job url.</summary>
        [HttpCheck("api/isalive")]
        public string Url { get; set; }
    }
}
