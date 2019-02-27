using Lykke.Job.FinancesAlerts.Settings.JobSettings;
using Lykke.Sdk.Settings;

namespace Lykke.Job.FinancesAlerts.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public FinancesAlertsJobSettings FinancesAlertsJob { get; set; }
    }
}
